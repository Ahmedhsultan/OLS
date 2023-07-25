using OLS.Persistence;
using OLS.Persistence.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OLS.UI
{
    public partial class AlignmentDataForm : Form
    {
        private Dictionary<Autodesk.Civil.DatabaseServices.Alignment, ComponentsContainer> map;
        public AlignmentDataForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            map = new Dictionary<Autodesk.Civil.DatabaseServices.Alignment, ComponentsContainer>();

            // Create a scroll pane
            Panel panel = new Panel();
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;

            // Add 10 sets of labels, combo boxes, and text boxes to the panel
            for (int i = 0; i < RunwayDB.getInstance().runwaysList.Count; i++)
            {
                createNewRowToPanel(panel, i);
            }

            // Add the panel to the form
            this.Alignments.Controls.Add(panel);

            // Set form properties
            this.Name = "AlignmentsData";
            this.Text = "Alignments Data";
        }

        private void createNewRowToPanel(Panel panel, int i)
        {
            Autodesk.Civil.DatabaseServices.Alignment alignment = RunwayDB.getInstance().runwaysList[i].alignment;

            // Create label 1 (Alignment name)
            Label label1 = new Label();
            label1.Text = alignment.Name + ":";
            label1.Location = new Point(10, 10 + i * 50);
            label1.AutoSize = true;

            // Create combo box with label
            Label label2 = new Label();
            label2.Text = "Profile:";
            label2.Location = new Point(label1.Right + 5, 10 + i * 50);
            label2.AutoSize = true;

            ComboBox profile = new ComboBox();
            profile.Location = new Point(label2.Right + 1, 10 + i * 50);
            foreach (Autodesk.Civil.DatabaseServices.Profile profile1 in RunwayDB.getInstance().runwaysList.FirstOrDefault(x => x.alignment == alignment).userInputs.allProfiles)
                profile.Items.Add(profile1.Name);
            profile.SelectedIndex = 0;
            // Subscribe to the SelectedIndexChanged event of the ComboBox
            profile.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            Runway runway = RunwayDB.getInstance().runwaysList.Find(x => x.alignment.Name == alignment.Name);
            Autodesk.Civil.DatabaseServices.Profile selectedProfile = runway.userInputs.allProfiles.Find(x => x.Name == profile.Text);

            // Create text box with label (Start Station)
            Label label3 = new Label();
            label3.Text = "Start Station:";
            label3.Location = new Point(profile.Right + 30, 10 + i * 50);
            label3.AutoSize = true;

            TextBox startStation = new TextBox();
            startStation.Location = new Point(label3.Right + 1, 10 + i * 50);
            startStation.Text = selectedProfile.StartingStation.ToString();

            // Create text box with label (End Station)
            Label label4 = new Label();
            label4.Text = "End Station:";
            label4.Location = new Point(startStation.Right + 30, 10 + i * 50);
            label4.AutoSize = true;

            TextBox endStation = new TextBox();
            endStation.Location = new Point(label4.Right + 1, 10 + i * 50);
            endStation.Text = selectedProfile.EndingStation.ToString();

            // Add controls to the panel
            panel.Controls.Add(label1);
            panel.Controls.Add(label2);
            panel.Controls.Add(profile);
            panel.Controls.Add(label3);
            panel.Controls.Add(startStation);
            panel.Controls.Add(label4);
            panel.Controls.Add(endStation);

            //Add to list of alignment components
            ComponentsContainer components = new ComponentsContainer();
            components.profile = profile;
            components.startStation = startStation;
            components.endStation = endStation;
            map.Add(alignment, components);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender; // Cast the sender object to a ComboBox
            string selectedItem = comboBox.SelectedItem.ToString();
            foreach(var item in map)
                if(comboBox == item.Value.profile)
                {
                    Runway runway = RunwayDB.getInstance().runwaysList.Find(x => x.alignment.Name == item.Key.Name);
                    Autodesk.Civil.DatabaseServices.Profile selectedProfile = runway.userInputs.allProfiles.Find(x => x.Name == comboBox.Text);
                    item.Value.startStation.Text = selectedProfile.StartingStation.ToString();
                    item.Value.endStation.Text = selectedProfile.EndingStation.ToString();
                }
        }

        private void AlignmentData_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Runway runway in RunwayDB.getInstance().runwaysList)
            {
                ComponentsContainer components = map[runway.alignment];
                runway.profile = runway.userInputs.allProfiles.Find(x => x.Name == components.profile.Text);
                runway.userInputs.startStation = double.Parse(components.startStation.Text);
                runway.userInputs.endStation = double.Parse(components.endStation.Text);
            }
            this.Close();
        }
    }
}
