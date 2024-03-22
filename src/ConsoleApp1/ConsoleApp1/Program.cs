using Microsoft.Build;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation.Context;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System.IO;
using System.Xml;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestSample();
            bool isNumber = false;
            int input = 0;
            Console.WriteLine("Input a positive number.");
            while (!isNumber) 
            {
                var value = Console.ReadLine();
                if (int.TryParse(value, out input ) && input > 0)
                {
                    isNumber = true;
                }

            }
            var workDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var templateFilePath = Path.Combine(workDir, "template", "testproj.csproj");
            string[] paths = InitializeProjects(templateFilePath, input);
            IssueSample(paths);
            Console.WriteLine("Done!");
        }

        static string[] InitializeProjects(string path, int number)
        {
            string projectName = Path.GetFileName(path);
            string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Guid.NewGuid().ToString());
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var files = Directory.EnumerateFiles(directory);
            if (files.Count() >= number)
            {
                return files.ToArray();
            }
            List<string> list = new List<string>();
            string content = File.ReadAllText(path);
            for (int i = 0; i < number; i++)
            {
                string createdFile = Path.Combine(directory, $"{projectName}{i.ToString()}");
                File.WriteAllText(createdFile, content);
                list.Add(createdFile);
            }
            return list.ToArray();
        }

        static void IssueSample(string[] paths)
        {
            var buildManager = new BuildManager();
            var projectCollection = new ProjectCollection();
            var context = EvaluationContext.Create(EvaluationContext.SharingPolicy.Shared);
            Parallel.ForEach(paths, new ParallelOptions { MaxDegreeOfParallelism = 4 }, path => {

                var xml = ProjectRootElement.Open(path, projectCollection, preserveFormatting: true);
                var project = Project.FromProjectRootElement(xml, new ProjectOptions
                {
                    //GlobalProperties = ...,
                    ProjectCollection = projectCollection,
                    //LoadSettings = ...,
                    EvaluationContext = context,
                    ToolsVersion = "Current"
                });
                var projectInstance = buildManager.GetProjectInstanceForBuild(project); // Exception here
                //projectInstance.Build();

            });
        }

        static void TestSample()
        {
            var buildManager = new BuildManager();
            var projectCollection = new ProjectCollection();
            var context = EvaluationContext.Create(EvaluationContext.SharingPolicy.Shared);
            var projectRootElement = ProjectRootElement.Create(projectCollection);
            var project = Project.FromProjectRootElement(projectRootElement, new ProjectOptions
            {
                //GlobalProperties = ...,
                ProjectCollection = projectCollection,
                //LoadSettings = ...,
                EvaluationContext = context
            });
            project.Build();
        }
    }
}
