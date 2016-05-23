using Addovation.Cloud.Apps.AddoResources.Client.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDUNv2.Models;
using System.IO;
using Windows.Storage;
using Newtonsoft.Json;
using SQLite.Net;

namespace IDUNv2.Services
{
    public class ReportService : IReportService
    {
        private static SQLiteConnection db = new SQLiteConnection(AppData.SqlitePlatform, AppData.DbPath);
        private CloudClient cloudClient;

        //private List<ReportTemplate> _templates = new List<ReportTemplate>
        //{
        //    new ReportTemplate { Name = "Template 1" },
        //    new ReportTemplate { Name = "Template 2" },
        //    new ReportTemplate { Name = "Template 3" },
        //};

        public ReportService()
        {
            this.cloudClient = AppData.CloudClient;
            db.CreateTable<ReportTemplate>();
            db.InsertOrReplace(new ReportTemplate { Id = 1, Name = "Template 1" });
            db.InsertOrReplace(new ReportTemplate { Id = 2, Name = "Template 2" });
            db.InsertOrReplace(new ReportTemplate { Id = 3, Name = "Template 3" });
        }

        public Task<List<ReportTemplate>> GetTemplates()
        {
            var templates = db.Table<ReportTemplate>().ToList();
            return Task.FromResult(templates);
        }

        public Task<ReportTemplate> SetTemplate(ReportTemplate template)
        {
            //int i = _templates.FindIndex(t => t.Name == template.Name);
            //if (i >= 0)
            //{
            //    _templates[i] = template;
            //}
            //else
            //{
            //    _templates.Add(template);
            //}
            //var t = db.Find<ReportTemplate>(template.Id);
            //int key = db.Insert(template);

            if (template.Id == 0)
            {
                db.Insert(template);
            }
            else
            {
                db.Update(template);
            }

            //var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Templates", CreationCollisionOption.OpenIfExists);
            //var file = await folder.CreateFileAsync(template.Name + ".json", CreationCollisionOption.ReplaceExisting);
            //var json = JsonConvert.SerializeObject(template);
            //await FileIO.WriteTextAsync(file, json);

            return Task.FromResult(template);
        }

        public async Task<List<FaultReport>> GetFaultReports()
        {
            return await cloudClient.GetFaultReports().ConfigureAwait(false);
        }

        public Task<FaultReport> SetFaultReport(FaultReport report)
        {
            throw new NotImplementedException();
        }
    }
}
