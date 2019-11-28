using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GreatQuotes.Data;
using GreatQuotes.ViewModels;

namespace GreatQuotes {

    public static class QuoteLoaderFactory
    {
        // This must be assigned to a method which creates a new quote loader.
        public static Func<IQuoteLoader> Create { get; set; }
    }
    public class QuoteLoader : IQuoteLoader
    {
        const string FileName = "quotes.xml";

        public IEnumerable<GreatQuoteViewModel> Load() {
            XDocument doc = null;

            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName);

            if (File.Exists(filename)) {
                try {
                    doc = XDocument.Load(filename);
                } catch {
                }
            }

            if (doc == null)
                doc = XDocument.Parse(DefaultData);

            if (doc.Root != null) {
                foreach (var entry in doc.Root.Elements("quote")) {
                    yield return new GreatQuoteViewModel(new GreatQuote(
                        entry.Attribute("author").Value,
                        entry.Value));
                }
            }
        }

        public void Save(IEnumerable<GreatQuoteViewModel> quotes) {
            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName);

            if (File.Exists(filename))
                File.Delete(filename);

            XDocument doc = new XDocument(
                new XElement("quotes",
                    quotes.Select(q =>
                        new XElement("quote", new XAttribute("author", q.Author)) {
                            Value = q.QuoteText
                        })));

            doc.Save(filename);
        }

        #region Internal Data
        static readonly string DefaultData =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<quotes>
    <quote author=""Eleanor Roosevelt"">Great minds discuss ideas; average minds discuss events; small minds discuss people.</quote>
    <quote author=""William Shakespeare"">Some are born great, some achieve greatness, and some have greatness thrust upon them.</quote>
    <quote author=""Winston Churchill"">All the great things are simple, and many can be expressed in a single word: freedom, justice, honor, duty, mercy, hope.</quote>
    <quote author=""Ralph Waldo Emerson"">Our greatest glory is not in never failing, but in rising up every time we fail.</quote>
    <quote author=""William Arthur Ward"">The mediocre teacher tells. The good teacher explains. The superior teacher demonstrates. The great teacher inspires.</quote>
</quotes>";
        #endregion
    }
    public interface IQuoteLoader
    {
        IEnumerable<GreatQuoteViewModel> Load();
        void Save(IEnumerable<GreatQuoteViewModel> quotes);
    }
    public class QuoteManager
    {
        private IQuoteLoader loader;
        private ITextToSpeech tts;

        //static readonly Lazy<QuoteManager> instance = new Lazy<QuoteManager>(() => new QuoteManager());
        //public static QuoteManager Instance { get => instance.Value; }
        public static QuoteManager Instance { get; private set; }

        //public IList<GreatQuoteViewModel> Quotes { get; set; }

        //readonly IQuoteLoader loader;
        public IList<GreatQuoteViewModel> Quotes { get; private set; }

        private QuoteManager()
        {
            loader = QuoteLoaderFactory.Create();
            Quotes = new ObservableCollection<GreatQuoteViewModel>(loader.Load());
        }
        
           
        public void Save()
        {
            loader.Save(Quotes);
        }
        
        
      
    public QuoteManager(IQuoteLoader loader, ITextToSpeech tts)
        {
            if (Instance != null)
            {
                throw new Exception("Can only create a single QuoteManager.");
            }
            
            Instance = this;
            this.loader = loader;
            this.tts = tts;
            Quotes = new ObservableCollection<GreatQuoteViewModel>(loader.Load());
        }
    public void SayQuote(GreatQuoteViewModel quote)
        {
            if (quote == null)
                throw new ArgumentNullException("No quote set");

            if (tts != null)
            {
                var text = quote.QuoteText;

                if (!string.IsNullOrWhiteSpace(quote.Author))
                    text += $" by {quote.Author}";

                tts.Speak(text);
            }
        }
    }
}
