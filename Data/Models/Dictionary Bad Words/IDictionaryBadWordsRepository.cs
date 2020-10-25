﻿using System.Linq;

namespace Data.Models.Dictionary_Bad_Words
{
    public interface IDictionaryBadWordsRepository
    {
        IQueryable<DictionaryBadWords> DictionaryBadWords { get; }

        void AddDictionaryBadWords(DictionaryBadWords badWord);

        void EditDictionaryBadWords(DictionaryBadWords badWord);

        void DeleteDictionaryBadWords(DictionaryBadWords badWord);

        void Save();
    }
}
