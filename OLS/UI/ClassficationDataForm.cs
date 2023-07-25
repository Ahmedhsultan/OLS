using System;
using System.Windows.Forms;
using OLS.Services.Classfications.Database.Classes;
using OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB;
using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Classes.TakeOffClass_DB;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.UI
{
    public partial class ClassficationDataForm : Form
    {
        private ITakeOff_Class takeOff_Class;

        public ClassficationDataForm()
        {
            takeOff_Class = new TakeOff_Class1();
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.comboBox2.Items.Add("Class A_1");
            this.comboBox2.Items.Add("Class A_2");
            this.comboBox2.Items.Add("Class A_3");
            this.comboBox2.Items.Add("Class A_4");
            this.comboBox2.Items.Add("Class B_1");
            this.comboBox2.Items.Add("Class B_2");
            this.comboBox2.Items.Add("Class B_3");
            this.comboBox2.Items.Add("Class C_1");
            this.comboBox2.Items.Add("Class C_2");
            this.comboBox2.Items.Add("Class D");

            this.comboBox1.Items.Add("Class 1");
            this.comboBox1.Items.Add("Class 2");
            this.comboBox1.Items.Add("Class 3");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CastumClass_DB CastumClass_DB = CastumClass_DB.getIntstance();
            try
            {
                CastumClass_DB.takeOffAttriputes = new TakeOffAttriputes()
                {
                    safeArea = double.Parse(takeoff_distance.Text),
                    innerEdge = double.Parse(takeoff_lengthofedge.Text),
                    totalLength = double.Parse(takeoff_totallength.Text),
                    finalWidth = double.Parse(takeoff_finalwidth.Text),
                    divargence = double.Parse(takeoff_divergence.Text),
                    slope = double.Parse(takeoff_slope.Text)
                };
                CastumClass_DB.landdingAttriputes = new LanddingAttriputes()
                {
                    safeArea = double.Parse(approach_distancefromthu.Text),
                    totalLength = double.Parse(approach_totallength.Text),
                    divargence = double.Parse(approach_divergence.Text),
                    innerEdge = double.Parse(approach_lengthofedge.Text),
                    s1 = double.Parse(approach_f_slope.Text),
                    l1 = double.Parse(approach_f_length.Text),
                    s2 = double.Parse(approach_s_slope.Text),
                    l2 = double.Parse(approach_s_length.Text),
                    s3 = 0.0,
                    l3 = double.Parse(approach_hl_length.Text)
                };
                CastumClass_DB.innerHorizontalAttriputes = new InnerHorizontalAttriputes()
                {
                    radius = double.Parse(innerhl_radius.Text),
                };
                CastumClass_DB.conicalAttriputes = new ConicalAttriputes()
                {
                    slope = double.Parse(conical_slope.Text),
                    height = double.Parse(conical_height.Text)
                };
                CastumClass_DB.transvareAttriputes = new TransvareAttriputes()
                {
                    slope = double.Parse(transitional_slope.Text)
                };

                this.Close();
            }
            catch (Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Input isnt valid");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedItem.ToString())
            {
                case "Class A_1":
                    addValuesToTextField_Approach(new ClassA_1_DB());
                    break;
                case "Class A_2":
                    addValuesToTextField_Approach(new ClassA_2_DB());
                    break;
                case "Class A_3":
                    addValuesToTextField_Approach(new ClassA_3_DB());
                    break;
                case "Class A_4":
                    addValuesToTextField_Approach(new ClassA_4_DB());
                    break;
                case "Class B_1":
                    addValuesToTextField_Approach(new ClassB_1_DB());
                    break;
                case "Class B_2":
                    addValuesToTextField_Approach(new ClassB_2_DB());
                    break;
                case "Class B_3":
                    addValuesToTextField_Approach(new ClassB_3_DB());
                    break;
                case "Class C_1":
                    addValuesToTextField_Approach(new ClassC_1_DB());
                    break;
                case "Class C_2":
                    addValuesToTextField_Approach(new ClassC_2_DB());
                    break;
                case "Class D":
                    addValuesToTextField_Approach(new ClassD_DB());
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Class 1":
                    addValuesToTextField_Approach_TakeOff(new TakeOff_Class1());
                    break;
                case "Class 2":
                    addValuesToTextField_Approach_TakeOff(new TakeOff_Class2());
                    break;
                case "Class 3":
                    addValuesToTextField_Approach_TakeOff(new TakeOff_Class3());
                    break;
            }
        }

        private void addValuesToTextField_Approach(IApproach_Class class_DB)
        {
            //Add Landding values
            approach_distancefromthu.Text = class_DB.landdingAttriputes.safeArea.ToString();
            approach_totallength.Text = class_DB.landdingAttriputes.totalLength.ToString();
            approach_divergence.Text = class_DB.landdingAttriputes.divargence.ToString();
            approach_lengthofedge.Text = class_DB.landdingAttriputes.innerEdge.ToString();
            approach_f_slope.Text = class_DB.landdingAttriputes.s1.ToString();
            approach_f_length.Text = class_DB.landdingAttriputes.l1.ToString();
            approach_s_slope.Text = class_DB.landdingAttriputes.s2.ToString();
            approach_s_length.Text = class_DB.landdingAttriputes.l2.ToString();
            approach_hl_length.Text = class_DB.landdingAttriputes.l3.ToString();


            //Add InnerHorizontal values
            innerhl_radius.Text = class_DB.innerHorizontalAttriputes.radius.ToString();


            //Add Conical values
            conical_slope.Text = class_DB.conicalAttriputes.slope.ToString();
            conical_height.Text = class_DB.conicalAttriputes.height.ToString();

            //Add Transvare values
            transitional_slope.Text = class_DB.transvareAttriputes.slope.ToString();
        }
        private void addValuesToTextField_Approach_TakeOff(ITakeOff_Class class_DB)
        {
            //Add takeoff values
            takeoff_distance.Text = class_DB.takeOffAttriputes.safeArea.ToString();
            takeoff_lengthofedge.Text = class_DB.takeOffAttriputes.innerEdge.ToString();
            takeoff_totallength.Text = class_DB.takeOffAttriputes.totalLength.ToString();
            takeoff_finalwidth.Text = class_DB.takeOffAttriputes.finalWidth.ToString();
            takeoff_divergence.Text = class_DB.takeOffAttriputes.divargence.ToString();
            takeoff_slope.Text = class_DB.takeOffAttriputes.slope.ToString();
        }
    }
}
