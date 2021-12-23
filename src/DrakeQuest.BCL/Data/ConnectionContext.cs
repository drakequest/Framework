namespace DrakeQuest.Data
{
	public class ConnectionContext
    {
        public string Name { get; }

        public string Environment { get; }

        public string Provider { get; }

        public string ConnectionString { get; }

        public ConnectionContext(string name,string environment, string provider, string connectionString)
        {
            Name = name;
            Environment = environment;
            Provider = provider;
            ConnectionString = connectionString;
        }

        #region methods

        public virtual ConnectionContext SetProvider(string provider)
        {
            return new ConnectionContext(this.Name, this.Environment, provider, this.ConnectionString);
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return $"{Environment} - {Name} - {Provider}";
        }

        public override bool Equals(object obj)
        {
            if (obj as ConnectionContext == null)
                return false;
            var _b = (ConnectionContext)obj;
            return $"{Environment} - {Name} - {Provider} - {ConnectionString}" == $"{_b.Environment} - {_b.Name} - {_b.Provider} - {_b.ConnectionString}";
        }

        public override int GetHashCode()
        {
            return ConnectionString.GetHashCode();
        }

        #endregion
    }
}
