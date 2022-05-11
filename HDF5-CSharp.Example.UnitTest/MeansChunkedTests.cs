using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HDF5CSharp.DataTypes;
using HDF5CSharp.Example;
using HDF5CSharp.Example.DataTypes;
using HDF5CSharp.Example.DataTypes.HDF5Store.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDF5_CSharp.Example.UnitTest
{
    [TestClass]
    public class MeansChunkedTests : BaseClass
    {

        [TestMethod]
        public async Task MeansChunkedTest()
        {
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameof(MeansChunkedTest)}.h5");
            Console.WriteLine(filename);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            kama = new KamaAcquisitionFile(filename, AcquisitionInterface.Simulator, Logger);
            ProcedureInfo info = new ProcedureInfo
            {
                Age = 18,
                FirstName = "Lior",
                LastName = "Banai",
                ExamDate = DateTime.Now,
                Procedure = "test",
                ProcedureID = "ID",
                Patient = new PatientInfo()
            };

            kama.SavePatientInfo(info);
            kama.UpdateSystemInformation("32423423", new[] { "11", "12" });
            kama.SetProcedureInformation(info);
            string data = File.ReadAllText(AcquisitionScanProtocolPath);
            AcquisitionProtocolParameters parameters = AcquisitionProtocolParameters.FromJson(data);
            await kama.StartLogging(parameters);
            var meansData = await GenerateMeans();
            kama.StopRecording();
            await kama.StopProcedure();


            using (KamaAcquisitionReadOnlyFile readFile = new KamaAcquisitionReadOnlyFile(filename))
            {
                readFile.ReadSystemInformation();
                readFile.ReadProcedureInformation();
                readFile.ReadPatientInformation();
                Assert.IsTrue(readFile.PatientInformation.Equals(kama.PatientInfo));
                Assert.IsTrue(readFile.ProcedureInformation.Equals(kama.ProcedureInformation));
                Assert.IsTrue(readFile.SystemInformation.Equals(kama.SystemInformation));

                var means = readFile.ReadMeansEvents();
                CheckMeans(meansData, means);
            }

            File.Delete(filename);
        }

        private void CheckMeans(List<(long timestamp, string data)> meansData, List<MeansFullECGEvent> means)
        {
            Assert.IsTrue(meansData.Count == means.Count);
            for (var i = 0; i < means.Count; i++)
            {
                MeansFullECGEvent mean = means[i];
                Assert.IsTrue(meansData[i].timestamp == mean.timestamp);
                Assert.IsTrue(meansData[i].data == mean.data);
                Assert.IsTrue(mean.index == i + 1);
            }
        }

        private async Task<List<(long, string)>> GenerateMeans()
        {
            var data = Enumerable.Range(0, 1000)
                .Select(i => (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), i.ToString())).ToList();

            var d1 = data.Take(10);
            foreach ((long timestamp, string data) d in d1)
            {
                kama.AppendMean(d.timestamp, d.data);
            }

            await Task.Delay(15000);
            var d2 = data.Skip(10).ToList();
            kama.AppendMeans(d2);
            return data;
        }
    }
}
