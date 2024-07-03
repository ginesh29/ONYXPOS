namespace Onyx_POS
{
    public class CommonSetting
    {
        public const string DEFAULTSELECT = "-- Select --";
        public const string ALLSELECT = "-- All --";
        public const string DisplayDateFormat = "dd/MM/yyyy";
        public const string InputDateFormat = "yyyy-MM-dd";
        public const string DeafultDate = "1900-01-01";
    }
    public class CommonMessage
    {
        public const string INSERTED = "added Successfully.";
        public const string UPDATED = "Updated Successfully";
        public const string DELETED = "Deleted Successfully";
        public const string SELECTROW = "Please select a record for Edit or Delete";
        public const string EMPTYGRID = "No data available";
        public const string INVALIDUSER = "Invalid Username or Password";
        public const string NOTHING = "Nothing to Display.";
        public const string SELECTONE = "Select one from the list";
        public const string USEREXISTS = "User already exists";
        public const string ITEMNOTFOUND = "Invalid barcode/item code!";
        public const string REMOTECONPROBLEM = "Could not connect to Remote Server in order to fetch Item and Group details";
        public const string INVALIDENTRY = "Invalid Data Entry";
        public const string CANNOTUPDATEADMINPASSWORD = "You can not update Admin Password";
        public const string CONFIRMPASSWORDNOTMATCHED = "Confirm Password not match with New Password";
    }
    public class ValidationMessage
    {
        public const string REQUIREDVALIDATION = "Please enter {0}";
        public const string REQUIREDSELECTVALIDATION = "Please select {0}";
        public const string REQUIREDFILEVALIDATION = "Please upload {0}";
        public const string PASSWORDMISMATCHVALIDATION = "{0} mismatch";
        public const string ENTERVALID = "Please enter valid {0}";
    }
}