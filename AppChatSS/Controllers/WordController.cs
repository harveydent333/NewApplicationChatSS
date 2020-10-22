using AppChatSS.Models.Dictionary_Bad_Words;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Controllers
{
    public class WordController : Controller
    {
        private static IDictionaryBadWordsRepository dictionaryBadWordsRepository;

        public WordController(IDictionaryBadWordsRepository dictionaryBadWordsRep)
        {
            dictionaryBadWordsRepository = dictionaryBadWordsRep;
        }

        [HttpGet]
        public IActionResult DictionaryBadWords()
        {
            ViewBag.badWords = dictionaryBadWordsRepository.DictionaryBadWords;
            return View("DictionaryBadWords");
        }

        [HttpPost]
        public IActionResult AddBadWord(DictionaryBadWords badWord)
        {
            if (ModelState.IsValid)
            {
                dictionaryBadWordsRepository.AddDictionaryBadWords(badWord);
                return RedirectToAction("DictionaryBadWords");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult EditBadWord(DictionaryBadWords badWord)
        {
            if (ModelState.IsValid)
            {
                dictionaryBadWordsRepository.EditDictionaryBadWords(badWord);
                return RedirectToAction("DictionaryBadWords");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult DeleteBadWord(DictionaryBadWords badWord)
        {
            dictionaryBadWordsRepository.DeleteDictionaryBadWords(badWord);
            return RedirectToAction("DictionaryBadWords");
        }
    }
}
