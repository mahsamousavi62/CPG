namespace CPG.Domain.SharedKernel.Helper
{
    public static class SharedServices
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }
    }
}
