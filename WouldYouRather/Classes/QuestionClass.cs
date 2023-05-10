using WouldYouRather.Classes;

namespace WouldYouRather
{
    public class QuestionClass : QuestionClass2
    {
        protected string option1;

        public QuestionClass(string option2, string option1) : base(option2)
        {
            this.option1 = option1;
        }
        public string GetOption1()
        {
            return this.option1;
        }
        public void SetOption1(string o)
        {
            this.option1 = o;
        }
        public string GetOption2()
        {
            return this.option2;
        }
        public void SetOption2(string o)
        {
            this.option2 = o;
        }
    }
}