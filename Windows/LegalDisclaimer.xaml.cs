using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCSimplified2.Windows
{
    /// <summary>
    /// Interaction logic for LegalDisclaimer.xaml
    /// </summary>
    public partial class LegalDisclaimer : Window
    {
        public LegalDisclaimer()
        {
            InitializeComponent();

            DisclaimerText.Text = "Legal Disclaimer for TC Simplified\n\n" +
                "1. General Information\n\n" +
                "TC Simplified is a software application designed to assist businesses in tracking employee work hours. The application is provided and managed" +
                " by [Your Company Name] (“Company,” “we,” “us,” or “our”). Use of TC Simplified implies acceptance of this Legal Disclaimer, and you are required" +
                " to comply with the terms provided herein.\n\n" +
                "2. No Legal Advice\n\n" +
                "The information provided through TC Simplified is for administrative and" +
                " operational purposes only and is not intended to be, nor should it be interpreted as, legal advice or a legal opinion. We are not a law firm," +
                " and we do not provide legal advice through this application. If you require legal advice, please consult with a qualified attorney.\n\n" +
                "3. Accuracy of Information\n\n" +
                "While we strive to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about" +
                " the completeness, accuracy, reliability, suitability, or availability with respect to TC Simplified or the information, products, services, or" +
                " related graphics contained on TC Simplified for any purpose. Any reliance you place on such information is therefore strictly at your own risk.\n\n" +
                "4. Compliance with Labor Laws\n\n" +
                "TC Simplified is designed to aid in tracking and managing employee hours, but it is your responsibility to ensure that your use of TC Simplified " +
                "complies with applicable local, state, national, and international laws, including but not limited to labor laws and regulations. We are not responsible" +
                " for ensuring that the application’s functionalities meet all legal requirements in your jurisdiction.\n\n" +
                "5. Limitation of Liability (see below for more details)\n\n" +
                "In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage " +
                "whatsoever arising from loss of data or profits arising out of, or in connection with, the use of TC Simplified.\n\n" +
                "6. Indemnification\n\n" +
                "You agree to indemnify, defend, and hold harmless the Company, its officers, directors, employees, agents, licensors, suppliers, and any third-party information " +
                "providers to TC Simplified from and against all losses, expenses, damages, and costs, including reasonable attorneys' fees, resulting from any violation" +
                " of this Disclaimer by you or any other person accessing TC Simplified using your account.\n\n" +
                "7. Modifications and Updates\n\n" +
                "The Company reserves the right to change the content and functionality of TC Simplified at any time without notice. This Disclaimer may be updated or modified from time to time," +
                " and such modifications will be effective immediately upon posting of the modified Disclaimer on TC Simplified. Your continued use of TC Simplified after" +
                " any such changes shall constitute your consent to such changes.\n\n\n\n";

            DisclaimerText.Text += "Limitation of Liability \n\n" +
                "TC Simplified (\"TC Simplified\") provides this Employee Time Card Application (\"TC Simplified\") as a tool to assist employers" +
                " and employees in tracking work hours, calculating regular and overtime pay, and managing payroll processes. While TC Simplified endeavors " +
                "to ensure the accuracy and reliability of the Application, the user acknowledges and agrees that there may be instances of errors or omissions" +
                " in the calculations performed by the Application, including but not limited to the calculations of taxes, holiday pay, overtime, and other" +
                " payroll-related figures. TC Simplified expressly disclaims any liability for errors or discrepancies that may arise in the course of using the Application," +
                " including any incorrect payroll calculations. It is the sole responsibility of the user to verify the accuracy of all calculations made by the Application" +
                " and to ensure compliance with all applicable laws and regulations. The user should consult with a professional accountant, payroll specialist, or legal advisor" +
                " to verify that all payroll calculations meet the statutory requirements. TC Simplified shall not be held liable for any losses, damages, costs, or expenses incurred" +
                " by the user or any third party arising directly or indirectly from the use of the Application, including but not limited to any errors in payroll calculations or" +
                " failure to comply with legal standards. The responsibility for ensuring accurate payroll processing, including the calculation of taxes, holiday pay, and overtime," +
                " rests solely with the user. By using this Application, the user agrees to indemnify and hold harmless TC Simplified from any claims, actions, legal proceedings, damages," +
                " liabilities, costs, and expenses, including attorneys' fees, arising from or related to the use of the Application. This disclaimer constitutes an integral part" +
                " of the terms of use of the Application. If you do not agree with any part of this disclaimer, you should not use this Application. \n\n" +
                "Governing Law\n\n" +
                "This disclaimer shall be governed by and construed in accordance with the laws of the United States. Any disputes arising out of or related to the use of the Application" +
                " shall be subject to the exclusive jurisdiction of the courts of the United States. \n\n" +
                "By using TC Simplified, you acknowledge that you have read, understood, and agree to be bound by this Disclaimer. If you" +
                " do not agree to these terms, do not use TC Simplified.\n\n\n\n" +
                "Contact Information \n\n" +
                "If you have any questions or concerns about this disclaimer, please contact us at me@example.com.";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
