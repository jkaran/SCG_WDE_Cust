/*
 Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using System.ComponentModel;
using System.Text;

namespace Pointel.CIS.Desktop.Core
{
    /// <summary>
    /// Comment: contains the properties that bind data in call info view
    /// Last Modified: 15-Feb-2015
    /// Created by: Pointel Inc
    /// </summary>
    public class MyListItem : IMyListItem, INotifyPropertyChanged
    {
        #region Properties

        private string optionName;

        public string OptionName
        {
            get
            {
                return optionName;
            }
            set
            {
                if (value != optionName) { optionName = value; }
            }
        }

        private string optionValue;

        public string OptionValue
        {
            get
            {
                return optionValue;
            }
            set
            {
                if (value != optionValue) { optionValue = value; }
            }
        }

        private string colorName;

        public string ColorName
        {
            get
            {
                return colorName;
            }
            set
            {
                if (value != colorName) { colorName = value; }
            }
        }

        #endregion Properties

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged Members

        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                txt.Append("OptionName = " + OptionName + ",\t");
                txt.Append("OptionValue = " + OptionValue + ",\t");
                txt.Append("ColorCode = " + ColorName + ",\t");
            }
            catch
            {
                return txt.ToString();
            }
            return txt.ToString();
        }
    }
}