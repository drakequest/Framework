#if NETCOREAPP
  using IBM.Data.DB2.Core;
#else
  using IBM.Data.DB2;
#endif

using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace DrakeQuest.Configuration
{
	/// <summary>
	/// Represents a configuration section containing the URLs and connection strings of several working environments.
	/// </summary>
	public static class EnvironmentsSectionRepository
  {
    /// <summary>
    /// Saves the configuration to the target database.
    /// </summary>
    public static void SaveConfigurationSection(this EnvironmentsSection environment)
    {
      using (var connection = new DB2Connection(environment.ConnectionString))
      {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
          Update(connection, "Applications", environment.Environments, transaction);
          Update(connection, "Applications", environment.Applications, transaction);
          transaction.Commit();
        }
      }

      using (var connection = new DB2Connection(environment.ConnectionString))
      {
        connection.Open();
        Update(connection, "Applications", environment.Applications);
      }
    }

    private static void Update<T>(DbConnection connection, string configKey, T configSection, DB2Transaction transaction = null)
    {

      var serializer = new XmlSerializer(typeof(T));
      var outStream = new StringWriter(CultureInfo.InvariantCulture);

      serializer.Serialize(outStream, configSection);
      var content = outStream.ToString();

      using (var command = connection.CreateCommand())
      {
        command.CommandType = CommandType.Text;
        if (transaction != null)
          command.Transaction = transaction;
        command.CommandText = "UPDATE NICK_TOOLS.CONFIGURATION SET XMLVALUE = ? WHERE KEY = ?";
        command.Parameters.Add(new DB2Parameter { Value = content });
        command.Parameters.Add(new DB2Parameter { Value = configKey });

        var result = command.ExecuteNonQuery();
        if (result == 0)
        {
          command.CommandText = "INSERT INTO NICK_TOOLS.CONFIGURATION(KEY, VALUE, XMLVALUE) VALUES (?, 'See XMLVALUE', ?)";
          command.Parameters.Clear();
          command.Parameters.Add(new DB2Parameter { Value = configKey });
          command.Parameters.Add(new DB2Parameter { Value = content });
          command.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <param name="environment">The connection string.</param>
    /// <param name="configKey">The connection string.</param>
    /// <returns></returns>

    public static  T LoadApplicationsConfiguration<T>(this EnvironmentsSection environment, string configKey)
    where T : class
    {
      T apps = null;
      using (var connection = new DB2Connection(environment.ConnectionString))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.Text;
          command.CommandText = "SELECT XMLVALUE FROM EUREKA_TOOLS.CONFIGURATION WHERE KEY = ?";
          command.Parameters.Add(new DB2Parameter { Value = configKey });

          var xmlConfiguration = command.ExecuteScalar();
          if (xmlConfiguration == null)
            return null;

          var stringConfiguration = xmlConfiguration.ToString();

          var deserializer = new XmlSerializer(typeof(T));
          var reader = new StringReader(stringConfiguration);
          apps = (T)deserializer.Deserialize(reader);
        }
      }

      return apps;
    }
  }
}
