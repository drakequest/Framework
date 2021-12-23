using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrakeQuest.Report
{
	public interface IEurekaAccess
	{
		int GetRowCount(string executionId, string datasetName);

		IEnumerable<string> GetParameterValues(string query);
	}
}
