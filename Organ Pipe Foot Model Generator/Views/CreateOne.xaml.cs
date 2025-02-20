﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ACadSharp;
using ACadSharp.IO;
using Microsoft.Win32;
using Organ_Pipe_Foot_Model_Generator.Entities;
using Organ_Pipe_Foot_Model_Generator.Logic;

namespace Organ_Pipe_Foot_Model_Generator.Views
{
    /// <summary>
    /// Interaction logic for CreateOne.xaml
    /// </summary>
    public partial class CreateOne : UserControl
    {
        private PipeFootTemplate _template;

        public CreateOne()
        {
            InitializeComponent();
        }

        #region Prevent input from being non numeric
        private void txbTopDiameter_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !InputValidationLogic.InputIsNumericOnly(e.Text);
        }

        private void txbBottomDiameter_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !InputValidationLogic.InputIsNumericOnly(e.Text);
        }

        private void txbHeight_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !InputValidationLogic.InputIsNumericOnly(e.Text);
        }

        private void txbMetalThickness_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !InputValidationLogic.InputIsNumericOnly(e.Text);
        }
        #endregion


        private void btnCalculatePipeFootMeasurements_Click(object sender, RoutedEventArgs e)
        {
            double topDiameter = double.Parse(txbTopDiameter.Text, CultureInfo.InvariantCulture);
            double bottomDiameter = double.Parse(txbBottomDiameter.Text, CultureInfo.InvariantCulture);
            double height = double.Parse(txbHeight.Text, CultureInfo.InvariantCulture);
            
            bool topAndBottomDiameterAreOuterDiameters = (bool)ckbIsOuterDiameter.IsChecked;

            if(!(bool)ckbIsOuterDiameter.IsChecked)
            {
                _template = new PipeFootTemplate(100, 100, topDiameter, bottomDiameter, height);
            }
            else
            {
                double metalThickness = double.Parse(txbMetalThickness.Text, CultureInfo.InvariantCulture);

                _template = new PipeFootTemplate(100, 100, topDiameter, bottomDiameter, height, metalThickness);
            }

            PipeFootMeasurements pipeFootMeasurements = _template.Measurements;

            // Display calculated measurements
            txbLengthSlantedSide.Text = pipeFootMeasurements.LengthSlantedSide.ToString();
            txbLengthInnerDiameter.Text = pipeFootMeasurements.LengthBottomDiameter.ToString();
            txbLengthOuterDiameter.Text = pipeFootMeasurements.LengthTopDiameter.ToString();
            txbSmallRadius.Text = pipeFootMeasurements.SmallRadius.ToString();
            txbLargeRadius.Text = pipeFootMeasurements.LargeRadius.ToString();
            txbCornerDegrees.Text = pipeFootMeasurements.CornerInDegrees.ToString();

            btnSaveModel.IsEnabled = true;
        }

        private void btnSaveModel_Click(object sender, RoutedEventArgs e)
        {
            // Save file for model
            string filePath = string.Empty;

            // Create a file to put the square in
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "DXF files (*.dxf)|*.dxf|All files (*.*)|*.*",
                Title = "Save a DXF File",
                FileName = "Pijp voet model.dxf" // Default file name
            };

            if ((bool)saveFileDialog.ShowDialog())
            {
                filePath = saveFileDialog.FileName; // Return the selected file path
            }

            // Add the insert into a document
            CadDocument doc = new CadDocument();

            _template.AddToCadDocument(doc);

            // Save the document using DxfWriter
            using (DxfWriter writer = new DxfWriter(filePath, doc, false))
            {
                writer.Write();
            }
        }

        private void ckbIsOuterDiameter_Click(object sender, RoutedEventArgs e)
        {
            bool IsChecked = (bool)ckbIsOuterDiameter.IsChecked;

            if (!IsChecked)
            {
                txbMetalThickness.IsEnabled = false;
            }
            else
            {
                txbMetalThickness.IsEnabled = true;
            }
        }
    }
}
