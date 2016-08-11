using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleBackup
{
    class BackupDefinitionReader
    {
        public BackupDefinition Read(string xmlDefinitionFilename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlDefinitionFilename);

            var definition = Read(xmlDocument);
            return definition;
        }

        public BackupDefinition Read(XmlDocument xmlDocument)
        {
            // Get the top-level document element ("SimpleBackup").
            var rootElement = xmlDocument.DocumentElement;

            // Locate the "Default" Definition node.
            int numberOfDefinitions = rootElement.ChildNodes.Count;
            XmlNode definitionElement = null;
            string definitionName = null;
            for (int i = 0; i < numberOfDefinitions; i++)
            {
                var testDefinitionElement = rootElement.ChildNodes[i];
                var testDefinitionName = testDefinitionElement.Attributes["Name"].Value;

                // We only support "Default" definition at the moment.
                if (testDefinitionName == "Default")
                {
                    definitionElement = testDefinitionElement;
                    definitionName = testDefinitionName;
                    break;
                }
            }

            // "Default" definition not found!
            if (definitionElement == null)
            {
                throw new InvalidOperationException(
                    string.Format("Error: Definition \"{0}\" not found in definition file.", "Default")
                    );
            }

            // Get destination root path.
            string destinationRootPath = definitionElement.Attributes["DestinationRootPath"].Value;

            // Get list of folder definitions.
            IList<BackupFolderDefinition> folderDefinitions = new List<BackupFolderDefinition>();
            foreach (var child in definitionElement.ChildNodes)
            {
                XmlElement folderElement = child as XmlElement;
                if (folderElement == null) continue;

                string sourceFolderPath = folderElement.Attributes["Source"].Value;
                string destinationFolderName = folderElement.Attributes["DestinationName"].Value;

                folderDefinitions.Add(new BackupFolderDefinition(sourceFolderPath, destinationFolderName));
            }

            // Create an object with all this info.
            var definition = new BackupDefinition(
                definitionName,
                destinationRootPath,
                folderDefinitions
                );

            return definition;
        }
    }
}
