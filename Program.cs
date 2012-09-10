using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DGE_scraper
{
    using System.Text;

    class Program 
    {
        private class Candidate
        {
            public string Name;
            public string Party;
            public int Votes;
        }

        private class Riding
        {
            public string Name;
            public int ValidVotes;
            public int RejectedVotes;
            public int RegisteredVoters;
            public IDictionary<string, Candidate> Candidates;
        }

        private class PartyComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                if (a == string.Empty)
                {
                    return b == string.Empty ? 0 : 1;
                }

                if (b == string.Empty)
                {
                    return -1;
                }

                if (a.StartsWith("IND"))
                {
                    return b.StartsWith("IND") ? a.CompareTo(b) : 1;
                }

                if (b.StartsWith("IND"))
                {
                    return -1;
                }

                return a.CompareTo(b);
            }
        }

        static void Main()
        {
            var ridings = GetRidings().ToArray();
            var partyNames = ridings.SelectMany(r => r.Candidates)
                                    .Select(kvp => kvp.Key)
                                    .Distinct()
                                    .OrderBy(p => p, new PartyComparer())
                                    .ToArray();

            var mep = ridings.Where(r => r.Candidates.ContainsKey(""));

            foreach (var riding in mep)
            {
                Debug.WriteLine(riding.Name);
            }

            WriteVoteCsv(ridings, partyNames);
            WriteCandidatesCsv(ridings, partyNames);
        }

        // Taken from http://monvote.qc.ca/fr/resultatsPreliminaires.asp
        private static readonly int [] ridingIds = new []{579, 573, 437, 373, 535, 323, 309, 293, 153, 303, 353, 525, 473, 713, 243, 431, 377, 129, 193, 593, 559, 619, 679, 173, 613, 763, 441, 659, 683, 433, 403, 483, 273, 759, 745, 443, 731, 557, 381, 133, 481, 387, 561, 149, 143, 733, 409, 623, 429, 643, 269, 361, 773, 603, 203, 183, 545, 779, 371, 209, 363, 423, 439, 583, 663, 289, 653, 399, 213, 407, 349, 465, 711, 103, 383, 451, 495, 233, 673, 419, 411, 329, 401, 123, 421, 553, 369, 563, 599, 753, 367, 253, 283, 703, 699, 413, 783, 379, 515, 567, 389, 447, 111, 393, 263, 189, 505, 417, 343, 177, 113, 163, 223, 633, 453, 333, 793, 229, 639, 169, 249, 397, 427, 449, 391};

        private static IEnumerable<Riding> GetRidings()
        {
            var client = new WebClient();

            foreach (var i in ridingIds)
            {
                using (var stream = client.OpenRead(string.Format("http://monvote.qc.ca/data/resultats/{0}.js", i)))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var content = reader.ReadToEnd();
                            yield return ParseRiding(content);
                        }
                    }
                }
            }
        } 

        private static readonly Regex ridingRegex = new Regex("^var Circ = \"(.+)\"", RegexOptions.Compiled|RegexOptions.Multiline);
        private static readonly Regex validVotesRegex = new Regex("^var VotesValTot = \"([0-9\\s]+)\"", RegexOptions.Compiled|RegexOptions.Multiline);
        private static readonly Regex rejectedVotesRegex = new Regex("^var VotesRejTot = \"([0-9\\s]+)\"", RegexOptions.Compiled|RegexOptions.Multiline);
        private static readonly Regex registeredVotersRegex = new Regex("^var NbElectInscr = \"([0-9\\s]+)\"", RegexOptions.Compiled|RegexOptions.Multiline);

        private static Riding ParseRiding(string str)
        {
            return new Riding
                {
                    Name = GetString(ridingRegex.Match(str)),
                    ValidVotes = GetInt(validVotesRegex.Match(str)),
                    RejectedVotes = GetInt(rejectedVotesRegex.Match(str)),
                    RegisteredVoters = GetInt(registeredVotersRegex.Match(str)),
                    Candidates = GetCandidateTable(str)
                };
        }

        private static string GetString(Match match)
        {
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static int GetInt(Match match)
        {
            return match.Success ? GetIntFromGroup(match.Groups[1]) : -1;
        }

        private static int GetIntFromGroup(Group group)
        {
            return int.Parse(string.Join(string.Empty, Regex.Replace(group.Value, @"\s", " ").Split(' ')));
        }

        private static readonly Regex candidateTableRegex = new Regex("^\\[\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\",\"(.*)\"]", RegexOptions.Compiled|RegexOptions.Multiline);
        private static IDictionary<string, Candidate> GetCandidateTable(string input)
        {
            var matches = candidateTableRegex.Matches(input);
            var indIdx = 1;
            return matches.Cast<Match>()
                          .Select(
                              match =>
                                  new Candidate
                                  {
                                      Name = string.Join(" ", match.Groups[1].Value.Split(',').Reverse()).Trim(),
                                      Party = match.Groups[2].Value != "IND" ? match.Groups[2].Value : "IND" + (indIdx++),
                                      Votes = GetIntFromGroup(match.Groups[3])
                                  })
                          .ToDictionary(c => c.Party, c => c);
        }

        private static void WriteVoteCsv(IEnumerable<Riding> ridings, IEnumerable<string> parties)
        {
            using(var stream = new StreamWriter("..\\..\\qc2012_resultats.csv",false,Encoding.UTF8))
            {
                // Write header
                stream.Write("Circonscription,Voteurs enregistrés,Votes rejetés");
                foreach (var party in parties)
                {
                    stream.Write("," + party);
                }
                stream.WriteLine();

                foreach (var riding in ridings)
                {
                    stream.Write(riding.Name);
                    stream.Write("," + riding.RegisteredVoters);
                    stream.Write("," + riding.RejectedVotes);
                    foreach (var party in parties)
                    {
                        stream.Write(",");
                        if (riding.Candidates.ContainsKey(party))
                        {
                            stream.Write(riding.Candidates[party].Votes);
                        }
                    }
                    stream.WriteLine();
                }
            }
        }

        private static void WriteCandidatesCsv(IEnumerable<Riding> ridings, IEnumerable<string> parties)
        {
            using (var stream = new StreamWriter("..\\..\\qc2012_candidats.csv", false, Encoding.UTF8))
            {
                // Write header
                stream.Write("Circonscription");
                foreach (var party in parties)
                {
                    stream.Write("," + party);
                }
                stream.WriteLine();

                foreach (var riding in ridings)
                {
                    stream.Write(riding.Name);
                    foreach (var party in parties)
                    {
                        stream.Write(",");
                        if (riding.Candidates.ContainsKey(party))
                        {
                            stream.Write(riding.Candidates[party].Name);
                        }
                    }
                    stream.WriteLine();
                }
            }
        }
    }
}
