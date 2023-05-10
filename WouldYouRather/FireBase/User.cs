using Android.Content;
using FbAuthentication;

namespace WouldYouRather
{
    public class User
    {
        private string name, mail, pwd, uid, pushCode, thirdGuestUid;
        private bool exist, isGuest;
        private readonly SP_data spd;

        public string Name { get => name; set => name = value; }
        public string Mail { get => mail; set => mail = value; }
        public string Pwd { get => pwd; set => pwd = value; }
        public bool Exist { get => exist; set => exist = value; }
        public bool IsGuest { get => isGuest; set => isGuest = value; }
        public string Uid { get => uid; set => uid = value; }
        public string ThirdGuestUid { get => thirdGuestUid; set => thirdGuestUid = value; }
        public string PushCode { get => pushCode; set => pushCode = value; }

        public User(string name, string mail, string pwd, bool exist)
        {
            this.Name = name.Trim();
            this.Mail = mail.Trim();
            this.Pwd = pwd.Trim();
            this.Exist = exist;
            this.Uid = null;
            this.PushCode = null;
            this.IsGuest = false;
            this.ThirdGuestUid = "";
        }
        public User(Context ctx)
        {
            spd = new SP_data(ctx);
            this.name = spd.GetStringValue(General.KEY_NAME);
            this.exist = this.name != string.Empty;
            if (this.exist)
            {
                this.mail = spd.GetStringValue(General.KEY_MAIL);
                this.pwd = spd.GetStringValue(General.KEY_PWD);

            }
        }

        internal bool Save()
        {
            bool success = spd.PutStringValue(General.KEY_NAME, this.name);
            success = success && spd.PutStringValue(General.KEY_PWD, this.pwd);
            return success && spd.PutStringValue(General.KEY_MAIL, this.mail);
        }
    }
}