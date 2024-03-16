namespace Chat.Application.ExtensionMethods
{
    public static class DateTimeExtention
    {
        public static int CalculateAge(this DateTime dateTime)
        {
            var today = DateTime.Today;
            var age=today.Year-dateTime.Year;
            if (dateTime.Date > today.AddYears(-age))
            {
                return age--;
            }

            return age;
        }
    }
}
