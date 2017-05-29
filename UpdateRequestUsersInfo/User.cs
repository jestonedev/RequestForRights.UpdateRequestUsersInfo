namespace UpdateUsersLogins
{
    public class User
    {
        public int IdUser { get; set; }
        public string Login { get; set; }
        public string Snp { get; set; }
        public string Post { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Unit { get; set; }
        public string Office { get; set; }

        public override bool Equals(object obj)
        {
            return this == obj as User;
        }

        protected bool Equals(User other)
        {
            return string.Equals(Login, other.Login) && 
                   string.Equals(Snp, other.Snp) && string.Equals(Post, other.Post) &&
                   string.Equals(Phone, other.Phone) && string.Equals(Department, other.Department) &&
                   string.Equals(Unit, other.Unit) && string.Equals(Office, other.Office);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Login != null ? Login.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Snp != null ? Snp.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Post != null ? Post.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Department != null ? Department.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Unit != null ? Unit.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Office != null ? Office.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(User first, User second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(User first, User second)
        {
            return !(first == second);
        }
    }
}
