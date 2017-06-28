using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Tests
{
    class TestPerson
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as TestPerson);
        }

        private bool Equals(TestPerson tp)
        {
            return (this.id == tp.id && this.name == tp.name && this.title == tp.title);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public int id;
        public string name;
        public string title;
    }

    public class TestLinq
    {
       private List<TestPerson> persons;

        public TestLinq(){
            persons = new List<TestPerson>();
            persons.Add(new TestPerson { id = 1,name="aaa",title="bbb"});
            persons.Add(new TestPerson { id = 2,name = "aaa2", title = "bbb2" });
        }

        //linq to dictionary
        public void ConvertToDict(){
            var q = (from s in persons
                     select new { s.id, s.name, s.title })
                    .ToDictionary(s => s.id + s.name, s => s.title);
            Dictionary<string, string> chnCheckNames = q; 
        }

        //contains(object)
        public void AddWithoutDuplicate()
        {
            TestPerson tp3 = new TestPerson { id = 2, name = "aaa2", title = "bbb2" };
            if (!persons.Contains(tp3)) persons.Add(tp3);
        }
    }
}
