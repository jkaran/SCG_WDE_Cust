namespace Pointel.CIS.Desktop.Core.NotePadView
{
    internal class ConsultNotepadViewModel : IConsultNotepadViewModel
    {
        public string Header
        {
            get
            {
                //LanguageDictionary dictionary = LanguageDictionary.GetDictionary(LanguageContext.Instance.Culture);
                //return dictionary.Translate("Windows.NotepadView.Header", "String", "Custom Note", typeof(string)) as string;
                return "Custom Note";
            }
        }
    }
}