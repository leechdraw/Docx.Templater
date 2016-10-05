using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Docx.Templater.TemplateCustomContent;
using Docx.Templater.Tests.Properties;
using FluentAssertions;
using NUnit.Framework;


namespace Docx.Templater.Tests
{
	[TestFixture]
	public class TemplateProcessorTests
	{
		[Test]
		public void FillingOneTableWithTwoRowsAndPreserveContentControls ()
		{
			//Given
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTable);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableFilledWithTwoRows);

			var valuesToFill = new Content (
							new []{new TableContent("Team Members", new[]
																		{
																				new TableRowContent( new []{new FieldContent( "Name", "Eric"),
																														 new FieldContent( "Title", "Program Manager")}),
																				new TableRowContent(new []{new FieldContent( "Name", "Bob"),
																														new FieldContent( "Title", "Developer")})
																		})});

			var template = new TemplateProcessor (templateDocument).FillContent (valuesToFill);

			//When
			var documentXml = template.Document.ToString ();

			//Then
			documentXml.Should ().Be (expectedDocument.Document.ToString ());
		}

		[Test]
		public void FillingOneTableWithTwoRowsAndRemoveContentControls ()
		{
			//Given
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTable);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableFilledWithTwoRowsAndRemovedCC);

			var valuesToFill = new Content (
					new []{
								new TableContent("Team Members", new[]
																				{
																						new TableRowContent
																								(new []{new FieldContent ( "Name", "Eric" ),
																								 new FieldContent ( "Title", "Program Manager" )}),
																						new TableRowContent
																								(new []{new FieldContent ( "Name", "Bob" ),
																								 new FieldContent ( "Title", "Developer" )})
																				})});

			//When
			var template = new TemplateProcessor (templateDocument)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			//Then
			expectedDocument.Document.ToString ().Should ().Be (documentXml);
		}

		[Test]
		public void FillingOneFieldWithValue ()
		{
			//Given
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleField);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldFilled);

			var valuesToFill = new Content (new [] { new FieldContent ("ReportDate", "09.06.2013") });

			//When
			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			//Then
			expectedDocument.Document.ToString ().Should ().Be (documentXml);
		}

		[Test]
		public void FillingOneFieldWithValue_ValueContainsLineBreak_ShouldInsertLineBreakToResultDocument ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleField);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldFilledWithLinebreaks);

			var dateTime = new DateTime (2013, 09, 06);
			var valuesToFill = new Content
			(new []{new FieldContent("ReportDate",
								string.Format("{0}\r\n{1}\n{2}",
								dateTime.ToString("d", CultureInfo.InvariantCulture),
								dateTime.ToString("D", CultureInfo.InvariantCulture),
								dateTime.ToString("y", CultureInfo.InvariantCulture))
						)});

			var template = new TemplateProcessor (templateDocument)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.IsNotNull (expectedDocument.Document);
			Assert.AreEqual (expectedDocument.Document.ToString (), documentXml);
		}

		[Test]
		public void FillingOneFieldWithValueAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleField);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldAndRemovedCC);

			var valuesToFill = new Content (new [] { new FieldContent ("ReportDate", "09.06.2013") });

			var template = new TemplateProcessor (templateDocument)
											.SetRemoveContentControls (true)
											.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneFieldWithWrongValue_WillNoticeWithWarning ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleField);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldWrongFilled);

			var valuesToFill = new Content (new [] { new FieldContent ("WrongReportDate", "09.06.2013") });

			var template = new TemplateProcessor (templateDocument)
											.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();
			var expectedXml = expectedDocument.ToString ();
			Assert.AreEqual (expectedXml, documentXml);
		}

		[Test]
		public void FillingOneFieldWithWrongValueAndDisabledErrorsNotifications_NotWillNoticeWithWarning ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleField);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldWrongFilledWithoutErrorsNotifications);

			var valuesToFill = new Content (new [] { new FieldContent ("WrongReportDate", "09.06.2013") });

			var template = new TemplateProcessor (templateDocument)
					.SetNoticeAboutErrors (false)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingFieldInTableHeaderWithValue ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithFieldInTableHeader);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithFieldInTableHeaderFilled);

			var valuesToFill = new Content (new IContentItem []{ new FieldContent ( "Count", "2" ),
										new TableContent("Team Members", new[]
												{
														new TableRowContent(new []{new FieldContent ( "Name", "Eric" ),
																				new FieldContent ( "Title", "Program Manager" )}),
														new TableRowContent(new []{new FieldContent ( "Name", "Bob" ),
																				new FieldContent ( "Title", "Developer" )}),
												})});

			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneTableWithTwoRowsWithWrongValues_WillNoticeWithWarning ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTable);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableWrongFilled);

			var valuesToFill = new Content (new []{
										new TableContent("Team Members", new[]
												{
														new TableRowContent(
																new []{
																				new FieldContent ( "Name", "Eric" ),
																				new FieldContent ( "Title", "Program Manager" ),
										new FieldContent( "WrongFieldName", "Value")}
																		),
														new TableRowContent(
																new []{
																				new FieldContent ( "Name", "Bob" ),
																				new FieldContent ( "Title", "Developer" ),
										new FieldContent( "WrongFieldName", "Value")}
																		),
												}
										)}
					);

			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneFieldWithSeveralTextEntries ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleFieldWithSeveralTextEntries);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleFieldWithSeveralTextEntriesFilled);

			var valuesToFill = new Content (new FieldContent ("ReportDate", "09.06.2013").AsArray ());

			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneTableWithAdjacentRows ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTableWithAdjacentRows);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableWithAdjacentRowsFilled);

			var valuesToFill = new Content (
							new TableContent ("Team Members", new []
									{
														new TableRowContent(
																new []{
																				new FieldContent ( "Name", "Eric" ),
																				new FieldContent ( "Title", "Program Manager" ),
										new FieldContent ( "Age", "33" ),
										new FieldContent ( "Gender", "Male" ),
										new FieldContent ( "Comment", "" )
}
																		),
														new TableRowContent(
																new []{
																				new FieldContent ( "Name", "Bob" ),
																				new FieldContent ( "Title", "Developer" ),
										new FieldContent ( "Age", "51" ),
										new FieldContent ( "Gender", "Male" ),
										new FieldContent ( "Comment", "Retiral" )
																		}),
									}
								 ).AsArray ()
					);

			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneTableWithMergedVerticallyRows ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTableWithMergedVerticallyRows);
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableWithMergedVerticallyRowsFilled);

			var valuesToFill = new Content (
					new TableContent ("Team Members")
							.AddRow (new FieldContent ("Name", "Eric").AsArray ())
							.AddRow (new FieldContent ("Name", "Bob").AsArray ()).AsArray ());

			var template = new TemplateProcessor (templateDocument)
					.FillContent (valuesToFill);

			var documentXml = template.Document.ToString ();

			Assert.AreEqual (expectedDocument.ToString (), documentXml);
		}

		[Test]
		public void FillingOneListAndPreserveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithSingleList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithSingleList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleListFilled_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithSingleListFilled_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithSingleListFilled_numbering);

			var valuesToFill = new Content (
							new ListContent ("Food Items",
									new []{
														new ListItemContent("Category", "Fruit"),
														new ListItemContent("Category", "Vegetables")}
									).AsArray ()
					);

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (expectedNumbering.ToString (), filledDocument.NumberingPart.ToString ());
		}

		[Test]
		public void FillingOneListAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithSingleList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithSingleList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleListFilledAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithSingleListFilledAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithSingleListFilledAndRemovedCC_numbering);

			var valuesToFill = new Content (new ListContent ("Food Items",
											 new []{new ListItemContent("Category", "Fruit"),
														 new ListItemContent("Category", "Vegetables")}).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (expectedNumbering.ToString (), filledDocument.NumberingPart.ToString ());
		}

		[Test]
		public void FillingOneListWithWrongValues_WillNoticeWithWarning ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithSingleList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithSingleList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleListWrongFilled_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithSingleListWrongFilled_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithSingleListWrongFilled_numbering);

			var valuesToFill = new Content (new ListContent ("Food Items", new ListItemContent ("WrongListItem", "Fruit").AsArray ()).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (expectedNumbering.ToString (), filledDocument.NumberingPart.ToString ());
		}

		[Test]
		public void FillingOneNestedListAndPreserveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleNestedList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithSingleNestedList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithSingleNestedList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleNestedListFIlled_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithSingleNestedListFIlled_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithSingleNestedListFIlled_numbering);

			var valuesToFill = new Content (
				 new ListContent ("Document",
						 new [] {
									 new ListItemContent("Header", "Introduction"),

										new ListItemContent("Header", "Chapter 1 - The new start screen")
												.AddContent(new FieldContent("Header text", "Header 2 paragraph text"))
												.AddNestedItem(new ListItemContent("Subheader", "What's new in Windows 8?")
														.AddContent(new FieldContent("Subheader text", "Subheader 2.1 paragraph text")))
												.AddNestedItem(new ListItemContent("Subheader", "Starting Windows 8")),

										new ListItemContent("Header", "Chapter 2 - The traditional Desktop")
												.AddNestedItem(new ListItemContent("Subheader", "Browsing the File Explorer"))
												.AddNestedItem(new ListItemContent("Subheader", "Getting the Lowdown on Folders and Libraries"))}).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (expectedNumbering.ToString (), filledDocument.NumberingPart.ToString ());
		}

		[Test]
		public void FillingOneNestedListAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleNestedList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithSingleNestedList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithSingleNestedList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleNestedListFilledAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithSingleNestedListFilledAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithSingleNestedListFilledAndRemovedCC_numbering);

			var valuesToFill = new Content (
					new ListContent ("Document")
							.AddItem (new ListItemContent ("Header", "Introduction"))

							.AddItem (new ListItemContent ("Header", "Chapter 1 - The new start screen")
									.AddContent (new FieldContent ("Header text", "Header 2 paragraph text"))
									.AddNestedItem (new ListItemContent ("Subheader", "What's new in Windows 8?")
											.AddContent (new FieldContent ("Subheader text", "Subheader 2.1 paragraph text")))
									.AddNestedItem (new ListItemContent ("Subheader", "Starting Windows 8")))

							.AddItem (new ListItemContent ("Header", "Chapter 2 - The traditional Desktop")
									.AddNestedItem (new ListItemContent ("Subheader", "Browsing the File Explorer"))
									.AddNestedItem (new ListItemContent ("Subheader", "Getting the Lowdown on Folders and Libraries"))).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (expectedNumbering.ToString (), filledDocument.NumberingPart.ToString ());
		}

		[Test]
		public void FillingOneNestedListInsideTableAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithNestedListInsideTableAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithNestedListInsideTableAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithNestedListInsideTableAndRemovedCC_numbering);

			var valuesToFill = new Content (
					new TableContent ("Products")
					.AddRow (
					new IContentItem []{
										new FieldContent("Category", "Fruits"),
										new ListContent("Items")
												.AddItem(new ListItemContent("Item", "Orange")
														.AddNestedItem(new ListItemContent("Color", "Orange")))
												.AddItem(new ListItemContent("Item", "Apple")
														.AddNestedItem(new ListItemContent("Color", "Green"))
														.AddNestedItem(new ListItemContent("Color", "Red")))})
					.AddRow (new IContentItem []{
										new FieldContent("Category", "Vegetables"),
										new ListContent("Items")
												.AddItem(new ListItemContent("Item", "Tomato")
														.AddNestedItem(new ListItemContent("Color", "Yellow"))
														.AddNestedItem(new ListItemContent("Color", "Red")))
												.AddItem(new ListItemContent("Item", "Cabbage"))}).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingOneNestedListInsideTableAndPreserveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithNestedListInsideTable_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithNestedListInsideTable_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithNestedListInsideTable_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithNestedListInsideTable_numbering);

			var valuesToFill = new Content (
					new TableContent ("Products")
					.AddRow (new IContentItem []{
										new FieldContent("Category", "Fruits"),
										new ListContent("Items")
												.AddItem(new ListItemContent("Item", "Orange")
														.AddNestedItem(new ListItemContent("Color", "Orange")))
												.AddItem(new ListItemContent("Item", "Apple")
														.AddNestedItem(new ListItemContent("Color", "Green"))
														.AddNestedItem(new ListItemContent("Color", "Red")))})
					.AddRow (new IContentItem []{
										new FieldContent("Category", "Vegetables"),
										new ListContent("Items")
												.AddItem(new ListItemContent("Item", "Tomato")
														.AddNestedItem(new ListItemContent("Color", "Yellow"))
														.AddNestedItem(new ListItemContent("Color", "Red")))
												.AddItem(new ListItemContent("Item", "Cabbage"))}).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingOneTableInsideListAndPreserveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithTableInsideList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithTableInsideList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithTableInsideList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithTableInsideList_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithTableInsideList_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithTableInsideList_numbering);

			var valuesToFill = new Content (
					new ListContent ("Products")
							.AddItem (new ListItemContent ("Category", "Fruits")
									.AddContent (new TableContent ("Items")
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Orange"), new FieldContent ("Count", "10") })
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Apple"), new FieldContent ("Count", "15") })))
							.AddItem (new ListItemContent ("Category", "Vegetables")
									.AddContent (new TableContent ("Items")
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Tomato"), new FieldContent ("Count", "8") })
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Cabbage"), new FieldContent ("Count", "17") }))).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingOneTableInsideListAndRemovedContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithTableInsideList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithTableInsideList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithTableInsideList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithTableInsideListAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithTableInsideListAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithTableInsideListAndRemovedCC_numbering);

			var valuesToFill = new Content (
					new ListContent ("Products")
							.AddItem (new ListItemContent ("Category", "Fruits")
									.AddContent (new TableContent ("Items")
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Orange"), new FieldContent ("Count", "10") })
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Apple"), new FieldContent ("Count", "15") })))
							.AddItem (new ListItemContent ("Category", "Vegetables")
									.AddContent (new TableContent ("Items")
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Tomato"), new FieldContent ("Count", "8") })
											.AddRow (new IContentItem [] { new FieldContent ("Name", "Cabbage"), new FieldContent ("Count", "17") }))).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingOneListAndFieldInsideNestedListAndPreserveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilled_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilled_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilled_numbering);

			var valuesToFill = new Content (
						 new ListContent ("Document")
								 .AddItem (new ListItemContent ("Header", "First classification")
										 .AddNestedItem (new ListItemContent ("Subheader", "Food classification")
												 .AddContent (new FieldContent ("Paragraph", "Text about food classification"))
												 .AddContent (new ListContent ("Products")
																 .AddItem (new ListItemContent ("Category", "Fruits")
																		 .AddNestedItem (new ListItemContent ("Name", "Apple"))
																		 .AddNestedItem (new ListItemContent ("Name", "Orange")))
																 .AddItem (new ListItemContent ("Category", "Vegetables")
																		 .AddNestedItem (new ListItemContent ("Name", "Tomato"))
																		 .AddNestedItem (new ListItemContent ("Name", "Cabbage"))))))
								 .AddItem (new ListItemContent ("Header", "Second classification")
										 .AddNestedItem (new ListItemContent ("Subheader", "Animals classification")
												 .AddContent (new FieldContent ("Paragraph", "Text about animal classification"))
												 .AddContent (
														 new ListContent ("Products")
																 .AddItem (new ListItemContent ("Category", "Vertebrate")
																		 .AddNestedItem (new ListItemContent ("Name", "Fish"))
																		 .AddNestedItem (new ListItemContent ("Name", "Mammal")))
																 .AddItem (new ListItemContent ("Category", "Invertebrate")
																		 .AddNestedItem (new ListItemContent ("Name", "Crustacean"))
																		 .AddNestedItem (new ListItemContent ("Name", "Insect")))))).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingOneListAndFieldInsideNestedListAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithFieldAndListInsideNestedList_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilledAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilledAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithFieldAndListInsideNestedListFilledAndRemovedCC_numbering);

			var valuesToFill = new Content (
						 new ListContent ("Document")
								 .AddItem (new ListItemContent ("Header", "First classification")
										 .AddNestedItem (new ListItemContent ("Subheader", "Food classification")
												 .AddContent (new FieldContent ("Paragraph", "Text about food classification"))
												 .AddContent (
														 new ListContent ("Products")
																 .AddItem (new ListItemContent ("Category", "Fruits")
																		 .AddNestedItem (new ListItemContent ("Name", "Apple"))
																		 .AddNestedItem (new ListItemContent ("Name", "Orange")))
																 .AddItem (new ListItemContent ("Category", "Vegetables")
																		 .AddNestedItem (new ListItemContent ("Name", "Tomato"))
																		 .AddNestedItem (new ListItemContent ("Name", "Cabbage"))))))
								 .AddItem (new ListItemContent ("Header", "Second classification")
										 .AddNestedItem (new ListItemContent ("Subheader", "Animals classification")
												 .AddContent (new FieldContent ("Paragraph", "Text about animal classification"))
												 .AddContent (
														 new ListContent ("Products")
																 .AddItem (new ListItemContent ("Category", "Vertebrate")
																		 .AddNestedItem (new ListItemContent ("Name", "Fish"))
																		 .AddNestedItem (new ListItemContent ("Name", "Mammal")))
																 .AddItem (new ListItemContent ("Category", "Invertebrate")
																		 .AddNestedItem (new ListItemContent ("Name", "Crustacean"))
																		 .AddNestedItem (new ListItemContent ("Name", "Insect")))))).AsArray ());

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingTwoTablesWithListsInsideAndPreverseContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilled_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilled_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilled_numbering);

			var valuesToFill = new Content (
					new IContentItem []{
							new TableContent("Peoples")
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Eric"),
											new FieldContent("Age", "34"),
											new ListContent("Childs")
													.AddItem(new ListItemContent("ChildName", "Robbie"))
													.AddItem(new ListItemContent("ChildName", "Trisha"))})
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Poll"),
											new FieldContent("Age", "40"),
											new ListContent("Childs")
													.AddItem(new FieldContent("ChildName", "Ann").AsArray())
													.AddItem(new FieldContent("ChildName", "Richard").AsArray())}),
							new TableContent("Team Members")
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Eric"),
											new ListContent("Roles")
													.AddItem(new ListItemContent("Role", "Developer"))
													.AddItem(new ListItemContent("Role", "Tester"))})
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Poll"),
											new ListContent("Roles")
													.AddItem(new FieldContent("Role", "Admin").AsArray())
													.AddItem(new FieldContent("Role", "Developer").AsArray())})});

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingTwoTablesWithListsInsideAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_document);
			var templateStyles = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_styles);
			var templateNumbering = XDocument.Parse (Resources.TemplateWithTwoTablesWithListsInside_numbering);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilledAndRemovedCC_document);
			var expectedStyles = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilledAndRemovedCC_styles);
			var expectedNumbering = XDocument.Parse (Resources.DocumentWithTwoTablesWithListsInsideFilledAndRemovedCC_numbering);

			var valuesToFill = new Content (
					new []{
							new TableContent("Peoples")
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Eric"),
											new FieldContent("Age", "34"),
											new ListContent("Childs")
													.AddItem(new FieldContent("ChildName", "Robbie").AsArray())
													.AddItem(new FieldContent("ChildName", "Trisha").AsArray())})
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Poll"),
											new FieldContent("Age", "40"),
											new ListContent("Childs")
													.AddItem(new FieldContent("ChildName", "Ann").AsArray())
													.AddItem(new FieldContent("ChildName", "Richard").AsArray())}),
							new TableContent("Team Members")
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Eric"),
											new ListContent("Roles")
													.AddItem(new FieldContent("Role", "Developer").AsArray())
													.AddItem(new FieldContent("Role", "Tester").AsArray())})
									.AddRow(
									new IContentItem[]{
											new FieldContent("Name", "Poll"),
											new ListContent("Roles")
													.AddItem(new FieldContent("Role", "Admin").AsArray())
													.AddItem(new FieldContent("Role", "Developer").AsArray())})});

			var filledDocument = new TemplateProcessor (templateDocument, templateStyles, templateNumbering)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
			Assert.AreEqual (expectedStyles.ToString (), filledDocument.StylesPart.ToString ());
			Assert.AreEqual (RemoveNsid (expectedNumbering.ToString ()), RemoveNsid (filledDocument.NumberingPart.ToString ()));
		}

		[Test]
		public void FillingTableWithTwoBlocksAndRemoveContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTableWithTwoBlocks);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableWithTwoBlocksFilledAndRemovedCC);

			var valuesToFill = new Content (
					new []{
												new TableContent("Team Members")
																.AddRow(new IContentItem[]{
																		new FieldContent("Name", "Eric"),
																		new FieldContent("Role", "Program Manager")})
																.AddRow(new IContentItem[]{
																		new FieldContent("Name", "Bob"),
																		new FieldContent("Role", "Developer")}),

																new TableContent("Team Members")
																.AddRow(new IContentItem[]{
																		new FieldContent("Statistics Role", "Program Manager"),
																		new FieldContent("Statistics Role Count", "1")})
																.AddRow(new IContentItem[]{
																		new FieldContent("Statistics Role", "Developer"),
																		new FieldContent("Statistics Role Count", "1")})});

			var filledDocument = new TemplateProcessor (templateDocument)
					.SetRemoveContentControls (true)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
		}

		[Test]
		public void FillingTableWithTwoBlocksAndPreverseContentControl ()
		{
			var templateDocument = XDocument.Parse (Resources.TemplateWithSingleTableWithTwoBlocks);

			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleTableWithTwoBlocksFilled);

			var valuesToFill = new Content (
									new []{new TableContent("Team Members")
																.AddRow(new IContentItem[]{
																		new FieldContent("Name", "Eric"),
																		new FieldContent("Role", "Program Manager")})
																.AddRow(new IContentItem[]{
																		new FieldContent("Name", "Bob"),
																		new FieldContent("Role", "Developer")}),

																new TableContent("Team Members")
																.AddRow(new IContentItem[]{
																		new FieldContent("Statistics Role", "Program Manager"),
																		new FieldContent("Statistics Role Count", "1")})
																.AddRow(new IContentItem[]{
																		new FieldContent("Statistics Role", "Developer"),
																		new FieldContent("Statistics Role Count", "1")})});

			var filledDocument = new TemplateProcessor (templateDocument)
					.SetRemoveContentControls (false)
					.FillContent (valuesToFill);

			Assert.AreEqual (expectedDocument.ToString (), filledDocument.Document.ToString ());
		}

		[Test]
		public void FillingSingleImageAndRemoveContentControl ()
		{
			var templateDocumentDocx = Resources.TemplateWithSingleImage;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleImage_AndRemovedCC);

			var newFile = File.ReadAllBytes ("Tesla.jpg");

			var valuesToFill = new Content (new ImageContent ("TeslaPhoto", newFile).AsArray ());

			TemplateProcessor processor;
			byte [] resultImage;
			using (var ms = new MemoryStream ()) {
				ms.Write (templateDocumentDocx, 0, templateDocumentDocx.Length);

				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (true)
						.FillContent (valuesToFill);

				resultImage = GetImageFromPart (processor, 0);
			}

			Assert.AreEqual (processor.ImagesPart.Count (), 1);
			Assert.IsNotNull (resultImage);
			Assert.IsTrue (resultImage.SequenceEqual (newFile));

			Assert.AreEqual (RemoveRembed (expectedDocument.ToString ().Trim ()),
					RemoveRembed (processor.Document.ToString ().Trim ()));
		}

		[Test]
		public void FillingSingleImageAndPreverseContentControl ()
		{
			var templateDocumentDocx = Resources.TemplateWithSingleImage;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleImage);

			var newFile = File.ReadAllBytes ("Tesla.jpg");

			var valuesToFill = new Content (new ImageContent ("TeslaPhoto", newFile).AsArray ());

			TemplateProcessor processor;
			byte [] resultImage;
			using (var ms = new MemoryStream ()) {
				ms.Write (templateDocumentDocx, 0, templateDocumentDocx.Length);
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (false)
						.FillContent (valuesToFill);

				resultImage = GetImageFromPart (processor, 0);
			}

			Assert.AreEqual (processor.ImagesPart.Count (), 1);
			Assert.IsNotNull (resultImage);
			Assert.IsTrue (resultImage.SequenceEqual (newFile));

			Assert.AreEqual (RemoveRembed (expectedDocument.ToString ().Trim ()),
					RemoveRembed (processor.Document.ToString ().Trim ()));
		}

		[Test]
		public void FillingSingleImage_ImageContentControlNotFound_ShowError ()
		{
			var templateDocumentDocx = Resources.TemplateEmpty;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithSingleImageNotFoundError);

			var newFile = File.ReadAllBytes ("Tesla.jpg");

			var valuesToFill = new Content (new ImageContent ("TeslaPhoto", newFile).AsArray ());

			TemplateProcessor processor;

			using (var ms = new MemoryStream (templateDocumentDocx)) {
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (false)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (processor.ImagesPart.Count (), 0);

			Assert.AreEqual (expectedDocument.ToString ().Trim (), processor.Document.ToString ().Trim ());
		}

		[Test]
		public void FillingImageInsideTable_CorrectFiledItems_Success ()
		{
			var templateDocumentDocx = Resources.TemplateWithImagesInsideTable;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithImagesInsideTable);

			var valuesToFill = new Content (
					new []{
								new TableContent("Scientists")
										.AddRow(
												new IContentItem[]{
												new FieldContent("Name", "Nicola Tesla"),
												new FieldContent("Born", new DateTime(1856, 7, 10).ToString("dd.MM.yyyy")),
												new ImageContent("Photo", File.ReadAllBytes("Tesla.jpg")),
												new FieldContent("Info",
														"Serbian American inventor, electrical engineer, mechanical engineer, physicist, and futurist best known for his contributions to the design of the modern alternating current (AC) electricity supply system")})
										.AddRow(new IContentItem[]{new FieldContent("Name", "Thomas Edison"),
												new FieldContent("Born", new DateTime(1847, 2, 11).ToString("dd.MM.yyyy")),
												new ImageContent("Photo", File.ReadAllBytes("Edison.jpg")),
												new FieldContent("Info","American inventor and businessman. He developed many devices that greatly influenced life around the world, including the phonograph, the motion picture camera, and the long-lasting, practical electric light bulb.")})
										.AddRow(new IContentItem[]{new FieldContent("Name", "Albert Einstein"),
												new FieldContent("Born", new DateTime(1879, 3, 14).ToString("dd.MM.yyyy")),
												new ImageContent("Photo", File.ReadAllBytes("Einstein.jpg")),
												new FieldContent("Info",
														"German-born theoretical physicist. He developed the general theory of relativity, one of the two pillars of modern physics (alongside quantum mechanics). Einstein's work is also known for its influence on the philosophy of science. Einstein is best known in popular culture for his mass–energy equivalence formula E = mc2 (which has been dubbed 'the world's most famous equation').")})
					});

			TemplateProcessor processor;

			using (var ms = new MemoryStream ()) {
				ms.Write (templateDocumentDocx, 0, templateDocumentDocx.Length);
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (true)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (3, processor.ImagesPart.Count ());

			Assert.AreEqual (RemoveRembed (expectedDocument.ToString ().Trim ()),
					RemoveRembed (processor.Document.ToString ().Trim ()));
		}

		[Test]
		public void FillingImageInsideAList_CorrectFiledItems_Success ()
		{
			var templateDocumentDocx = Resources.TemplateWithImagesInsideList;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithImagesInsideListFilledAndRemovedCC);

			var valuesToFill = new Content (
				new []{
								new ListContent("Scientists")
									.AddItem(
									new IContentItem[]{
									new FieldContent("Name", "Nicola Tesla"),
											new ImageContent("Photo", File.ReadAllBytes("Tesla.jpg")),
											new FieldContent("Dates of life", string.Format("{0}-{1}",
													1856, 1943)),
											new FieldContent("Info",
													"Serbian American inventor, electrical engineer, mechanical engineer, physicist, and futurist best known for his contributions to the design of the modern alternating current (AC) electricity supply system")})
									.AddItem(
									new IContentItem[]{
									new FieldContent("Name", "Thomas Edison"),
											new ImageContent("Photo", File.ReadAllBytes("Edison.jpg")),
											new FieldContent("Dates of life", string.Format("{0}-{1}",
													1847, 1931)),
											new FieldContent("Info",
													"American inventor and businessman. He developed many devices that greatly influenced life around the world, including the phonograph, the motion picture camera, and the long-lasting, practical electric light bulb.")})
									.AddItem(
									new IContentItem[]{
									new FieldContent("Name", "Albert Einstein"),
											new ImageContent("Photo", File.ReadAllBytes("Einstein.jpg")),
											new FieldContent("Dates of life", string.Format("{0}-{1}",
													1879, 1955)),
											new FieldContent("Info",
													"German-born theoretical physicist. He developed the general theory of relativity, one of the two pillars of modern physics (alongside quantum mechanics). Einstein's work is also known for its influence on the philosophy of science. Einstein is best known in popular culture for his mass–energy equivalence formula E = mc2 (which has been dubbed 'the world's most famous equation').")})
		});

			TemplateProcessor processor;

			using (var ms = new MemoryStream ()) {
				ms.Write (templateDocumentDocx, 0, templateDocumentDocx.Length);
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (true)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (3, processor.ImagesPart.Count ());

			Assert.AreEqual (RemoveRembed (expectedDocument.ToString ().Trim ()),
					RemoveRembed (processor.Document.ToString ().Trim ()));
		}

		[Test]
		public void FillingFieldsInHeaderAndFooter_WithCorrectValues_Success ()
		{
			var templateDocumentDocx = Resources.TemplateEmptyWithFieldsInHeaderAndFooter;
			var expectedHeader = XDocument.Parse (Resources.DocumentWithFieldFilledInHeaderAndFooter_header);
			var expectedFooter = XDocument.Parse (Resources.DocumentWithFieldFilledInHeaderAndFooter_footer);

			var valuesToFill = new Content (new []{
								new FieldContent("Company name", "Spiderwasp Communications"),
								new FieldContent("Copyright", "© All rights reserved")});

			TemplateProcessor processor;

			using (var ms = new MemoryStream (templateDocumentDocx)) {
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (false)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (processor.HeaderParts.Count, 1);
			Assert.AreEqual (processor.FooterParts.Count, 1);

			Assert.AreEqual (expectedHeader.ToString ().Trim (), processor.HeaderParts.First ().Value.ToString ().Trim ());
			Assert.AreEqual (expectedFooter.ToString ().Trim (), processor.FooterParts.First ().Value.ToString ().Trim ());
		}

		[Test]
		public void FillingFieldsInHeaderAndFooter_WithCorrectValuesAndRemoveContentControls_Success ()
		{
			var templateDocumentDocx = Resources.TemplateEmptyWithFieldsInHeaderAndFooter;
			var expectedHeader = XDocument.Parse (Resources.DocumentWithFieldFilledInHeaderAndFooterAndRemovedCC_header);
			var expectedFooter = XDocument.Parse (Resources.DocumentWithFieldFilledInHeaderAndFooterAndRemovedCC_footer);

			var valuesToFill = new Content (
					new []{
								new FieldContent("Company name", "Spiderwasp Communications"),
								new FieldContent("Copyright", "© All rights reserved")});

			TemplateProcessor processor;

			using (var ms = new MemoryStream (templateDocumentDocx)) {
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (true)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (processor.HeaderParts.Count, 1);
			Assert.AreEqual (processor.FooterParts.Count, 1);

			Assert.AreEqual (expectedHeader.ToString ().Trim (), processor.HeaderParts.First ().Value.ToString ().Trim ());
			Assert.AreEqual (expectedFooter.ToString ().Trim (), processor.FooterParts.First ().Value.ToString ().Trim ());
		}

		[Test]
		public void FillingTwoLists_InMainDocumentAndInFooter_Success ()
		{
			var templateDocumentDocx = Resources.TemplateWithTwoListsInMainDocumentAndInFooter;
			var expectedDocument = XDocument.Parse (Resources.DocumentWithTwoListsInMainDocumentAndInFooter_document);
			var expectedFooter = XDocument.Parse (Resources.DocumentWithTwoListsInMainDocumentAndInFooter_footer);

			var valuesToFill = new Content (
					new []{
								new ListContent("Footer",
										new []{  new ListItemContent("Footer item", "Spiderwasp Communications"),
														 new ListItemContent("Footer item", "© All rights reserved")}),
										new ListContent("Document",
												new []{
										new ListItemContent("Header", "Introduction"),
										new ListItemContent("Header", "Chapter 1 - The new start screen")
												.AddNestedItem(new ListItemContent("Subheader", "What's new in Windows 8?"))
												.AddNestedItem(new ListItemContent("Subheader", "Starting Windows 8")),
										new ListItemContent("Header", "Chapter 2 - The traditional Desktop")
												.AddNestedItem(new ListItemContent("Subheader", "Browsing the File Explorer"))
												.AddNestedItem(new ListItemContent("Subheader", "Getting the Lowdown on Folders and Libraries"))})
					});

			TemplateProcessor processor;

			using (var ms = new MemoryStream (templateDocumentDocx)) {
				processor = new TemplateProcessor (ms)
						.SetRemoveContentControls (false)
						.FillContent (valuesToFill);
			}

			Assert.AreEqual (processor.FooterParts.Count, 1);

			Assert.AreEqual (expectedDocument.ToString ().Trim (), processor.Document.ToString ().Trim ());
			Assert.AreEqual (expectedFooter.ToString ().Trim (), processor.FooterParts.First ().Value.ToString ().Trim ());
		}

		private static byte [] GetImageFromPart (TemplateProcessor processor, int partIndex)
		{
			if (!processor.ImagesPart.Any ())
				return null;
			var stream = processor.ImagesPart.ToArray () [partIndex].GetStream ();

			var resultImage = new byte [stream.Length];
			using (var reader = new BinaryReader (processor.ImagesPart.First ().GetStream ())) {
				reader.Read (resultImage, 0, (int)stream.Length);
			}
			return resultImage;
		}

		private static string RemoveNsid (string source)
		{
			const string nsidRegexp = "nsid w:val=\"[0-9a-fA-F]+\"";
			return Regex.Replace (source, nsidRegexp, "");
		}

		private static string RemoveRembed (string source)
		{
			const string rembedRegexp = "r:embed=\"[0-9a-zA-Z]+\"";
			return Regex.Replace (source, rembedRegexp, "");
		}
	}
}