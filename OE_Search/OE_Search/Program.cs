using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OE_Search
{
    internal class Program
    {
        static Dictionary<string, List<string>> oeCodes = new Dictionary<string, List<string>>();
        static List<string> glids = new List<string>();
        static void Main(string[] args)
        {
            string connectionString = "server=131.0.0.199;port=3307;uid=robi;database=gerisondump";
            MySqlConnection dumpConn = new MySqlConnection();
            dumpConn.ConnectionString = connectionString;
            dumpConn.Open();

            connectionString = "server=131.0.1.92;port=3306;uid=robi;database=tcs_javitas";
            MySqlConnection fixConn = new MySqlConnection();
            fixConn.ConnectionString = connectionString;
            fixConn.Open();

            RequestBadGlids(dumpConn);
            foreach (string glid in glids)
            {
                if (CheckAgainstDatabase(fixConn, glid)) continue;
                EvaluateGlid(dumpConn, fixConn, glid);
            }
        }

        static void EvaluateGlid(MySqlConnection conn, MySqlConnection localConn, string glid)
        {
            Console.Clear(); Console.WriteLine("\x1b[3J");
            Console.WriteLine(glid);
            List<string> oeCodes = new List<string>();
            MySqlCommand command = new MySqlCommand($"SELECT oe FROM t8007 t WHERE t.glid='{glid}'", conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    oeCodes.Add(reader["oe"].ToString());
                }
            }

            List<string> glidCodes = new List<string>();
            foreach (string oeCode in oeCodes)
            {
                command = new MySqlCommand($"SELECT glid FROM t8007 t WHERE t.oe='{oeCode}'", conn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        glidCodes.Add(reader["glid"].ToString());
                    }
                }
            }

            Dictionary<string, string> tcsCodes = new Dictionary<string, string>();
            Dictionary<string, int> tcsCodeOccurance = new Dictionary<string, int>();
            foreach (string glidCode in glidCodes)
            {
                command = new MySqlCommand($"SELECT tcs,tcscode FROM t1004 t WHERE t.glid='{glidCode}'", conn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["tcscode"].ToString() == "QD") continue;
                        if (tcsCodes.ContainsKey(reader["tcscode"].ToString()))
                        {
                            tcsCodeOccurance[reader["tcscode"].ToString()] = tcsCodeOccurance[reader["tcscode"].ToString()] + 1;
                        }
                        else
                        {
                            tcsCodes.Add(reader["tcscode"].ToString(), reader["tcs"].ToString());
                            tcsCodeOccurance.Add(reader["tcscode"].ToString(), 1);
                        }
                    }
                }
            }

            if (tcsCodes.Count == 0)
                SetGlidTcs(localConn, glid, "QD");
            if (tcsCodes.Count == 1)
                SetGlidTcs(localConn, glid, tcsCodes.Values.First());

            List<KeyValuePair<string, int>> sortedDict = tcsCodeOccurance.ToList();
            sortedDict.Sort(
                delegate(
                    KeyValuePair<string, int> pair1,
                    KeyValuePair<string, int> pair2
                )
                {
                    return pair1.Value.CompareTo( pair2.Value );
                }
            );

            if ((float)sortedDict[0].Value / (float)sortedDict[1].Value > 0.6f)
            {
                int selection = 0;
                Console.WriteLine("Válassz tcs-t (Nem meghatározotthoz írj: -1)");
                int i = 0;
                foreach (var item in tcsCodes)
                {
                    Console.WriteLine($"{i} -> {item.Value}");
                    i++;
                }
                selection = int.Parse( Console.ReadLine() );
                if (selection == -1) { SetGlidTcs(localConn, glid, "QD"); return; }
                SetGlidTcs(localConn, glid, sortedDict[selection].Key);
            }
        }

        static void SetGlidTcs(MySqlConnection conn, string glid, string tcscode)
        {
            MySqlCommand command = new MySqlCommand($"INSERT INTO wv_oe (glid, tcscode) VALUES ({glid}, {tcscode}) ON DUPLICATE KEY UPDATE tcscode=VALUES(tcscode)", conn);
            command.ExecuteReader();
        }

        static bool CheckAgainstDatabase(MySqlConnection conn, string glid)
        {
            MySqlCommand command = new MySqlCommand($"SELECT glid,tcscode FROM vw_oe WHERE glid='{glid}'", conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["tcscode"].ToString() != "QD") return true;
                }
            }
            return false;
        }

        static void RequestBadGlids(MySqlConnection conn)
        {
            Console.WriteLine("Populating glid list...");
            MySqlCommand command = new MySqlCommand(@"SELECT glid FROM t1004 t WHERE t.gyto='VW OE' AND t.tcscode='QD'", conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    glids.Add(reader["glid"].ToString());
                }
            }
        }
    }
}
