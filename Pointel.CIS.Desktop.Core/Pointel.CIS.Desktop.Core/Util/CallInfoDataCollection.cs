using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Pointel.CIS.Desktop.Core.Util
{
    public class CallInfoDataCollection
    {
        public string ConnId { get; set; }

        public ObservableCollection<IMyListItem> CallInfoData = new ObservableCollection<IMyListItem>();

        public ObservableCollection<IMyListItem> AccountData = new ObservableCollection<IMyListItem>();

        public ObservableCollection<IMyListItem> AddressData = new ObservableCollection<IMyListItem>();

        public ObservableCollection<IMyListItem> PhoneData = new ObservableCollection<IMyListItem>();

        public ObservableCollection<IMyListItem> IVRData = new ObservableCollection<IMyListItem>();

        public ObservableCollection<IMyListItem> NoticeAmountData = new ObservableCollection<IMyListItem>();

        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                txt.Append(ConnId);
                txt.Append("\n *************CallInfoData*************\n");
                foreach (IMyListItem item in CallInfoData)
                {
                    txt.Append(item.ToString() + "\n");
                }
                txt.Append("\n *************AccountData************* \n");
                foreach (IMyListItem item in AccountData)
                {
                    txt.Append(item.ToString() + "\n");
                }
                txt.Append("\n *************AddressData************* \n");
                foreach (IMyListItem item in AddressData)
                {
                    txt.Append(item.ToString() + "\n");
                }
                txt.Append("\n *************PhoneData************* \n");
                foreach (IMyListItem item in PhoneData)
                {
                    txt.Append(item.ToString() + "\n");
                }
                txt.Append("\n *************IVRData************* \n");
                foreach (IMyListItem item in IVRData)
                {
                    txt.Append(item.ToString() + "\n");
                }
                txt.Append("\n *************NoticeAmountData************* \n");
                foreach (IMyListItem item in NoticeAmountData)
                {
                    txt.Append(item.ToString() + "\n");
                }
            }
            catch (Exception)
            {
                return txt.ToString();
            }
            return txt.ToString();
        }
    }
}