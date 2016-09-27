﻿using Docx.Templater.TemplateCustomContent;
using System;
using System.IO;

namespace Docx.Templater.Example
{
    internal static class Program
    {
        private static void Main()
        {
            File.Delete("OutputDocument.docx");
            File.Copy("InputTemplate.docx", "OutputDocument.docx");

            var valuesToFill = new Content(
                new IContentItem[]{
                // Add field.
                                           new FieldContent("Report date", DateTime.Now.ToShortDateString()),

                                           // Add field in header.
                                           new FieldContent("Company name", "Spiderwasp Communications"),

                                           // Add field in footer.
                                           new FieldContent("Copyright", "© All rights reserved"),

                                           // Add table.
                                           new TableContent("Team Members Table")
                                               .AddRow(
                                                       new[]
                                                           {
                                                               new FieldContent("Name", "Eric"),
                                                               new FieldContent("Role", "Program Manager")
                                                           }
                                                      )
                                               .AddRow(new[]
                                                           {
                                                               new FieldContent("Name", "Bob"),
                                                               new FieldContent("Role", "Developer")
                                                           }),

                                           // Add field inside table that not to propagate.
                                           new FieldContent("Count", "2"),
                // Add list.
                                           new ListContent("Team Members List")
                                               .AddItem(new[]
                                                            {
                                                                new FieldContent("Name", "Eric"),
                                                                new FieldContent("Role", "Program Manager")
                                                            })
                                               .AddItem(new[]
                                                            {
                                                                new FieldContent("Name", "Bob"),
                                                                new FieldContent("Role", "Developer")
                                                            }),

                                           // Add nested list.
                                           new ListContent("Team Members Nested List")
                                               .AddItem(new ListItemContent("Role", "Program Manager")
                                                            .AddNestedItem(new FieldContent("Name", "Eric"))
                                                            .AddNestedItem(new FieldContent("Name", "Ann")))
                                               .AddItem(new ListItemContent("Role", "Developer")
                                                            .AddNestedItem(new FieldContent("Name", "Bob"))
                                                            .AddNestedItem(new FieldContent("Name", "Richard"))),

                                           // Add list inside table.
                                           new TableContent("Projects Table")
                                               .AddRow(new IContentItem[]{
                                                           new FieldContent("Name", "Eric"),
                                                           new FieldContent("Role", "Program Manager"),
                                                           new ListContent("Projects")
                                                               .AddItem(new[]
                                                                            {new FieldContent("Project", "Project one")})
                                                               .

                                                               AddItem(new[]{new FieldContent("Project", "Project two")
            } )})
        .

        AddRow(new IContentItem[]{
                            new FieldContent("Name", "Bob"),
                            new FieldContent("Role", "Developer"),
                            new ListContent("Projects")
                                .AddItem(new IContentItem[]{new FieldContent("Project", "Project one")})
                                .AddItem(new IContentItem[]{new FieldContent("Project", "Project three")})
                                }
                            ),

                // Add table inside list.
                new ListContent("Projects List")
                    .AddItem(new ListItemContent("Project", "Project one")
                        .AddContent(new TableContent("Team members")
                            .AddRow(new[]{
                                    new FieldContent("Name", "Eric"),
                                    new FieldContent("Role", "Program Manager")})
                            .AddRow(new[]{
                                    new FieldContent("Name", "Bob"),
                                    new FieldContent("Role", "Developer")})))
                    .AddItem(new ListItemContent("Project", "Project two")
                        .AddContent(new TableContent("Team members")
                            .AddRow(new[]{
                                    new FieldContent("Name", "Eric"),
                                    new FieldContent("Role", "Program Manager")})))
                    .AddItem(new ListItemContent("Project", "Project three")
                        .AddContent(new TableContent("Team members")
                            .AddRow(new[]{
                                    new FieldContent("Name", "Bob"),
                                    new FieldContent("Role", "Developer")}))),

                // Add table with several blocks.
                new TableContent("Team Members Statistics")
                    .AddRow(new[]{
                            new FieldContent("Name", "Eric"),
                            new FieldContent("Role", "Program Manager")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Richard"),
                            new FieldContent("Role", "Program Manager")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Bob"),
                            new FieldContent("Role", "Developer")}),

                    new TableContent("Team Members Statistics")
                    .AddRow(new[]{
                            new FieldContent("Statistics Role", "Program Manager"),
                            new FieldContent("Statistics Role Count", "2")})
                    .AddRow(new[]{
                            new FieldContent("Statistics Role", "Developer"),
                            new FieldContent("Statistics Role Count", "1")}),

                // Add table with merged rows
                new TableContent("Team members info")
                    .AddRow(new[]{
                            new FieldContent("Name", "Eric"),
                            new FieldContent("Role", "Program Manager"),
                            new FieldContent("Age", "37"),
                            new FieldContent("Gender", "Male")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Bob"),
                            new FieldContent("Role", "Developer"),
                            new FieldContent("Age", "33"),
                            new FieldContent("Gender", "Male")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Ann"),
                            new FieldContent("Role", "Developer"),
                            new FieldContent("Age", "34"),
                            new FieldContent("Gender", "Female")}),

                // Add table with merged columns
                new TableContent("Team members projects")
                    .AddRow(new[]{
                            new FieldContent("Name", "Eric"),
                            new FieldContent("Role", "Program Manager"),
                            new FieldContent("Age", "37"),
                            new FieldContent("Projects", "Project one, Project two")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Bob"),
                            new FieldContent("Role", "Developer"),
                            new FieldContent("Age", "33"),
                            new FieldContent("Projects", "Project one")})
                    .AddRow(new[]{
                            new FieldContent("Name", "Ann"),
                            new FieldContent("Role", "Developer"),
                            new FieldContent("Age", "34"),
                            new FieldContent("Projects", "Project two")}),

                // Add image
                new ImageContent("photo", File.ReadAllBytes("Tesla.jpg")),

                // Add images inside a table
                new TableContent("Scientists Table")
                    .AddRow(
                    new IContentItem[]{
                        new FieldContent("Name", "Nicola Tesla"),
                            new FieldContent("Born", new DateTime(1856, 7, 10).ToShortDateString()),
                            new ImageContent("Photo", File.ReadAllBytes("Tesla.jpg")),
                            new FieldContent("Info",
                                "Serbian American inventor, electrical engineer, mechanical engineer, physicist, and futurist best known for his contributions to the design of the modern alternating current (AC) electricity supply system")})
                    .AddRow(new IContentItem[]{new FieldContent("Name", "Thomas Edison"),
                            new FieldContent("Born", new DateTime(1847, 2, 11).ToShortDateString()),
                            new ImageContent("Photo", File.ReadAllBytes("Edison.jpg")),
                            new FieldContent("Info",
                                "American inventor and businessman. He developed many devices that greatly influenced life around the world, including the phonograph, the motion picture camera, and the long-lasting, practical electric light bulb.")})
                    .AddRow(new IContentItem[]{new FieldContent("Name", "Albert Einstein"),
                            new FieldContent("Born", new DateTime(1879, 3, 14).ToShortDateString()),
                            new ImageContent("Photo", File.ReadAllBytes("Einstein.jpg")),
                            new FieldContent("Info",
                                "German-born theoretical physicist. He developed the general theory of relativity, one of the two pillars of modern physics (alongside quantum mechanics). Einstein's work is also known for its influence on the philosophy of science. Einstein is best known in popular culture for his mass–energy equivalence formula E = mc2 (which has been dubbed 'the world's most famous equation').")}),

                // Add images inside a list
                new ListContent("Scientists List")
                  .AddItem(new IContentItem[]{new FieldContent("Name", "Nicola Tesla"),
                          new ImageContent("Photo", File.ReadAllBytes("Tesla.jpg")),
                          new FieldContent("Dates of life", string.Format("{0}-{1}",
                              1856, 1943)),
                          new FieldContent("Info",
                              "Serbian American inventor, electrical engineer, mechanical engineer, physicist, and futurist best known for his contributions to the design of the modern alternating current (AC) electricity supply system")})
                  .AddItem(new IContentItem[]{new FieldContent("Name", "Thomas Edison"),
                          new ImageContent("Photo", File.ReadAllBytes("Edison.jpg")),
                          new FieldContent("Dates of life", string.Format("{0}-{1}",
                              1847, 1931)),
                          new FieldContent("Info",
                              "American inventor and businessman. He developed many devices that greatly influenced life around the world, including the phonograph, the motion picture camera, and the long-lasting, practical electric light bulb.")})
                  .AddItem(
                    new IContentItem[]{new FieldContent("Name", "Albert Einstein"),
                          new ImageContent("Photo", File.ReadAllBytes("Einstein.jpg")),
                          new FieldContent("Dates of life", string.Format("{0}-{1}",1879, 1955)),
                          new FieldContent("Info","German-born theoretical physicist. He developed the general theory of relativity, one of the two pillars of modern physics (alongside quantum mechanics). Einstein's work is also known for its influence on the philosophy of science. Einstein is best known in popular culture for his mass–energy equivalence formula E = mc2 (which has been dubbed 'the world's most famous equation').")})
                }
            );

            using (var outputDocument = new TemplateProcessor("OutputDocument.docx")
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
        }
    }
}