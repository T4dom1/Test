using Assecor_Test.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Assecor_Test.Controllers
{
    public class ColorsController : ApiController
    {
        public List<PersonInfo> Get(string color)
        {
            var personController = new PersonController();
            var peopleList = personController.FindPeople();
            var searchedPerson = personController.FilterPeople(color, peopleList);
            return searchedPerson;
        }
    }

    public class PersonController : ApiController
    {

        private string fileUrl = "https://raw.githubusercontent.com/Assecor-GmbH/assecor-assessment-backend/master/sample-input.csv";//Die Url wo sich die Datei zum auslesen befindet
        private String[] colorTable =
        {
            "blau",
            "grün",
            "violett",
            "rot",
            "gelb",
            "türkis",
            "weiß"
        };//Dient zur Umwandlung der Zahlen zu einer Farbe

        /* GET: api/People
         * Diese Methode wird bei einem Standart GetRequest ohne key ausgeführt
         */
        public IEnumerable<PersonInfo> Get()
        {

            var peopleList = FindPeople();

            return peopleList;
        }

        /* GET: api/People/5
         * Diese Methode ist für das Get Request mit dem key 'id' zuständig
         * @id wird benutzt um die Person mit der Id zu suchen und dann anzuzeigen
         */
        public PersonInfo Get(int id)
        {
            var peopleList = FindPeople();
            var seachedPerson = FilterPeople(id, peopleList);
            return seachedPerson;
        }

        /* GET: api/People/color
         * Diese Methode ist für das Get Request mit dem key 'color' zuständig
         * @color wird benutzt um die Personen mit der Lieblingsfarbe zu suchen und anzuzeigen
         */
        public List<PersonInfo> Get(string color)
        {
            var peopleList = FindPeople();
            var searchedPerson = FilterPeople(color, peopleList);
            return searchedPerson;
        }


        #region Get People
        /* Diese Methode wird benutzt um die Personen aus der Datei zu laden */
        public List<PersonInfo> FindPeople()
        {
            var allPeople = new List<PersonInfo>();
            try
            {
                List<string> lines = new List<string>();
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(fileUrl);
                    HttpWebResponse rep = (HttpWebResponse)req.GetResponse();

                    StreamReader sr = new StreamReader(rep.GetResponseStream());
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                    }
                    lines.Add("" + sr.EndOfStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                for (int i = 1; i <= lines.Count; i++)
                {
                    string[] fields = lines[i].Split(',');

                    if (fields.Length == 4)
                    {
                        allPeople.Add(new PersonInfo()
                        {
                            id = i,
                            name = fields[1].Trim(),
                            nachname = fields[0].Trim(),
                            zipcode = fields[2].Trim().Substring(0, 5),
                            city = fields[2].Trim().Substring(5),
                            color = colorTable[int.Parse(fields[3])]
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
            }


            return allPeople;
        }
        #endregion

        #region Get Data
        /* Diese Methode wird benutzt um die Daten aus der Datei zu lesen
         * @return results wird benutzt um die daten aus der Datei zurück zu geben
         */
        public string GetData(string url)
        {
            var results = " ";

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(rep.GetResponseStream());
                results = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        #endregion

        #region Filter People
        /* Diese Methode wird benutzt um eine Liste nach der Person mit der Id zu durchsuchen und zurück zu geben
         *  @id dient zur identifikation der gesuchten Person aus der Liste
         * @return searchedPerson gibt die Person wieder die mithilfe der Id gesucht wurde
         */
        public PersonInfo FilterPeople(int id, List<PersonInfo> list)
        {
            var searchedPerson = new PersonInfo();
            for (int i = 1; i <= list.Count; i++)
            {
                if (i == id)
                {
                    searchedPerson = list[i - 1];
                }
            }
            return searchedPerson;
        }

        /* Diese Methode wird benutzt um eine Liste nach der Person mit der Id zu durchsuchen und zurück zu geben
         * @color dient zur filterung der Liste nach den Personen deren lieblingsfarbe die farbe des Parameters ist
         * @return searchedPerson gibt die Personen wieder die mithilfe der color gesucht wurden
         */
        public List<PersonInfo> FilterPeople(string color, List<PersonInfo> list)
        {
            var searchedPeople = new List<PersonInfo>();
            for (int i = 1; i <= list.Count; i++)
            {
                if (list[i - 1].color.Equals(color))
                {
                    searchedPeople.Add(list[i - 1]);
                }
            }
            return searchedPeople;
        }
        #endregion
    }
}
