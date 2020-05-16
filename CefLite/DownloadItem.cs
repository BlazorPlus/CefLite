using System;
using System.Collections.Generic;
using System.Text;

using CefLite.Interop;

namespace CefLite
{
    public class DownloadItem
    {
        static public long DataVersion { get; private set; }
        static public event Action DataVersionUpdated;

        static bool _postingUpdate = false;
        static public void PostVersionUpdateEvent()
        {
            if (_postingUpdate) return;
            _postingUpdate = true;
            CefWin.PostToAppThread(delegate
            {
                DataVersion++;
                DataVersionUpdated?.Invoke();
            });
        }

        static public Action<System.Windows.Forms.Form> ShowDownloadFormHandler { get; set; }
        static public void ShowDownloadForm(System.Windows.Forms.Form parentForm)
        {
            if (ShowDownloadFormHandler != null)
                ShowDownloadFormHandler(parentForm);
            else
                ShowDefaultDownloadForm(parentForm);
        }

        static DefaultDownloadForm _defdlf;
        static public void ShowDefaultDownloadForm(System.Windows.Forms.Form parentForm)
        {
            if (_defdlf == null || _defdlf.IsDisposed)
            {
                _defdlf = new DefaultDownloadForm();
                _defdlf.Show(parentForm);
            }
            else
            {
                CefWin.ActivateForm(parentForm);
            }
        }

        static public DownloadItem[] Items
        {
            get
            {
                lock (List)
                    return List.ToArray();
            }
        }

        static internal List<DownloadItem> List = new List<DownloadItem>();

        static internal void Show(CefDownloadItem item, CefDownloadItemCallback callback)
        {
            DownloadItem ditem = new DownloadItem();
            ditem._item = item;
            ditem._callback = callback;
            lock (List)
                List.Add(ditem);
            PostVersionUpdateEvent();
        }

        CefDownloadItem _item;
        CefDownloadItemCallback _callback;

        public void Cancel()
        {
            if (IsInProgress)
                _callback.Cancel();
            PostVersionUpdateEvent();
        }
        public void RemoveFromList()
        {
            if (IsInProgress)
                _callback.Cancel();
            lock (List)
                List.Remove(this);
            PostVersionUpdateEvent();
        }



        public DateTime StartTime { get; } = DateTime.Now;

        public bool IsValid => _item.IsValid;
        public bool IsInProgress => _item.IsInProgress;
        public bool IsCanceled => _item.IsCanceled;
        public bool IsComplete => _item.IsComplete;
        public long Id => _item.Id;
        public long PercentComplete => _item.PercentComplete;
        public long CurrentSpeed => _item.CurrentSpeed;
        public long TotalBytes => _item.TotalBytes;
        public long ReceivedBytes => _item.ReceivedBytes;

        public string SuggestedFileName => _item.SuggestedFileName;

        public string FullPath => _item.FullPath;
        public string Url => _item.Url;
        public string OriginalUrl => _item.OriginalUrl;
        public string MimeType => _item.MimeType;
        public string ContentDisposition => _item.ContentDisposition;

    }

}
