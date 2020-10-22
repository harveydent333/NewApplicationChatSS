using System.Linq;

namespace AppChatSS.Models.Dictionary_Bad_Words
{
    public class DictionaryBadWordsRepository : IDictionaryBadWordsRepository
    {
        private ApplicationDbContext dictionaryBadWordsContext;

        public DictionaryBadWordsRepository(ApplicationDbContext context)
        {
            dictionaryBadWordsContext = context;
        }

        /// <summary>
        /// Возвращает коллекцию записей словаря плохих слов
        /// </summary>
        public IQueryable<DictionaryBadWords> DictionaryBadWords => dictionaryBadWordsContext.DicrtionaryBadWords;

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddDictionaryBadWords(DictionaryBadWords badWord)
        {
            dictionaryBadWordsContext.DicrtionaryBadWords.Add(badWord);
            Save();
        }

        /// <summary>
        /// Метод изменяет запись в таблице базы данных
        /// </summary>
        public void EditDictionaryBadWords(DictionaryBadWords badWord)
        {
            dictionaryBadWordsContext.Update(badWord);
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteDictionaryBadWords(DictionaryBadWords badWord)
        {
            dictionaryBadWordsContext.DicrtionaryBadWords.Remove(badWord);
            Save();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public void Save()
        {
            dictionaryBadWordsContext.SaveChanges();
        }
    }
}