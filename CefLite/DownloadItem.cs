using System;
using System.Collections.Generic;
using System.Linq;
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
				_postingUpdate = false;
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
			ShowOrUpdate(item, callback, true);
		}
		static internal void Update(CefDownloadItem item, CefDownloadItemCallback callback)
		{
			ShowOrUpdate(item, callback, false);
		}

		static internal void ShowOrUpdate(CefDownloadItem item, CefDownloadItemCallback callback,bool create)
		{
			var id = item.Id;
			var ditem = List.Where(v => v.Id == id).FirstOrDefault();
			if (ditem == null)
			{
				if (!create)
					return;

				ditem = new DownloadItem();
				lock (List)
					List.Add(ditem);
			}
			else
			{
			}
			ditem.SetFrom(item);
			ditem._callback = callback;
			PostVersionUpdateEvent();
		}

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

		void SetFrom(CefDownloadItem item)
		{
			Id = item.Id;
			IsValid = item.IsValid;
			IsInProgress = item.IsInProgress;
			IsCanceled = item.IsCanceled;
			IsComplete = item.IsComplete;
			Id = item.Id;
			PercentComplete = item.PercentComplete;
			CurrentSpeed = item.CurrentSpeed;
			TotalBytes = item.TotalBytes;
			ReceivedBytes = item.ReceivedBytes;

			SuggestedFileName = item.SuggestedFileName;

			FullPath = item.FullPath;
			Url = item.Url;
			OriginalUrl = item.OriginalUrl;
			MimeType = item.MimeType;
			ContentDisposition = item.ContentDisposition;
		}


		public DateTime StartTime { get; } = DateTime.Now;

		public uint Id { get; private set; }
		public bool IsValid { get; private set; }
		public bool IsInProgress { get; private set; }
		public bool IsCanceled { get; private set; }
		public bool IsComplete { get; private set; }
		public long PercentComplete { get; private set; }
		public long CurrentSpeed { get; private set; }
		public long TotalBytes { get; private set; }
		public long ReceivedBytes { get; private set; }

		public string SuggestedFileName { get; private set; }

		public string FullPath { get; private set; }
		public string Url { get; private set; }
		public string OriginalUrl { get; private set; }
		public string MimeType { get; private set; }
		public string ContentDisposition { get; private set; }

	}

}
