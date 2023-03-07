using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CsProject{
    class Program{
        static void Main(string[] arg){
            //
            List<Staff> myStaff = new List<Staff>();
            Filereader fr = new Filereader();
            int month =0;
            int year = 0;
            while (year==0){
                Console.Write("Please enter the year: ");
                try{
                    year = Convert.ToInt32(Console.ReadLine());
                }catch(FormatException){
                    Console.Write("Please enter a valid year: ");
                }
            }
            while (month==0){
                Console.Write("Please enter the month: ");
                try{
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month<1 || month >12){
                        month = 0;
                        Console.WriteLine("Please enter a valid Month(between 1 to 12)");
                        }
                }catch(Exception e){
                    Console.WriteLine("Please enter a valid Month: ");
                }
            }
            myStaff=fr.ReadFile();
            for(int i =0; i<myStaff.Count; i++){
                try{
                    Console.Write("Enter hours worked for: {0} ", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());

                }catch(Exception e){
                    Console.WriteLine("Please enter a valid hours: ");
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();
        }
    }
    class Staff{
        //
        private float hoursRate;
        private int hWorked;
        public string NameOfStaff{get; private set;}
        public float TotalPay{get; protected set;}
        public float BasicPay{get; private set;}

        public int HoursWorked{
            get{
                return hWorked;
            }
            set{
                if (value>0){
                    hWorked = value;
                }else{
                    hWorked = 0;
                }
            }
        }
        public Staff(string name, float rate){
            NameOfStaff = name;
            hoursRate = rate;
        }
        public virtual void CalculatePay(){
            Console.WriteLine("Calculating the pay...");
            BasicPay = hWorked*hoursRate;
            TotalPay = BasicPay;
        }
        public override string ToString()
        {
            return "Name of the staff: "+NameOfStaff+", Hourly rate: "+hoursRate+", Worked hours: "+hWorked;
        }

    }
    class Manager : Staff{
        //
        private const float managerHourlyRate = 50;
        public int Allowance{get; private set;}
        public Manager(string name) : base(name, managerHourlyRate){}
        public override void CalculatePay(){
            base.CalculatePay();
            Allowance = 1000;
            if (HoursWorked>160){
                TotalPay+=Allowance;
            }
        }
        public override string ToString()
        {
            return "\nNameOfStaff = " + NameOfStaff + "\nmanagerHourlyRate= "
+ managerHourlyRate + "\nHoursWorked = " + HoursWorked +
"\nBasicPay = "
+ BasicPay + "\nAllowance = " + Allowance + "\n\nTotalPay = " +
TotalPay;
        }

    }
    class Admin : Staff{
        //
        private const float overtimeRate = 15.5f;
        private const float adminHourlyrate = 30f;
        public float Overtime{get; private set;}

        public Admin(string name) : base(name, adminHourlyrate){

        }
        public override void CalculatePay(){
            base.CalculatePay();
            if (HoursWorked>160){
                Overtime = overtimeRate*(HoursWorked-160);
                TotalPay += Overtime;
            }
        }
        public override string ToString()
        {
            return "\nNameOfStaff = " + NameOfStaff
+ "\nadminHourlyRate = " + adminHourlyrate + "\nHoursWorked = "
+ HoursWorked
+ "\nBasicPay = " + BasicPay + "\nOvertime = " + Overtime
+ "\n\nTotalPay = " + TotalPay;;
        }

    }
    class Filereader{
        //
        public List<Staff> ReadFile(){
            List<Staff> myStaff = new List<Staff>();
            string path = "staff.txt";
            string[] result = new string[2];
            string[] separator = {", "};
            if (File.Exists(path)){
                using (StreamReader sr = new StreamReader(path)){
                while (sr.EndOfStream!=true){
                    result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (result[1]=="Manager"){
                        myStaff.Add(new Manager(result[0]));
                    }else if(result[1]=="Admin"){
                        myStaff.Add(new Admin(result[0]));
                    }
                }
                sr.Close();
            }
            }else{
                Console.WriteLine("Error, No such file");
            }
            return myStaff;
            
        }
        
    }
    class PaySlip{
        //
        private int month;
        private int year;
        enum MonthOfYear{
            Jan=1,
            Feb=2, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
        }
        public PaySlip(int payMonth, int payYear){
            month = payMonth;
            year = payYear;
        }
        public void GeneratePaySlip(List<Staff> myStaff){
            string path;
            foreach(Staff f in myStaff){
                path = f.NameOfStaff+".txt";
                using(StreamWriter sw = new StreamWriter(path, true)){
                    sw.WriteLine("Payslip for {0} {1}", (MonthOfYear)month, year);
                    sw.WriteLine("-------------------------");
                    sw.WriteLine("Name of the Staff: "+ f.NameOfStaff);
                    sw.WriteLine("Hours worked: "+ f.HoursWorked);
                    sw.WriteLine("-------------------------");
                    sw.WriteLine("Basic Pay: "+ f.BasicPay);
                    if(f.GetType()==typeof(Manager)){sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);}
                    else if(f.GetType()==typeof(Admin)){sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);}
                    sw.WriteLine("-------------------------");
                    sw.WriteLine("Totoal Pay: "+ f.TotalPay);
                    sw.WriteLine("-------------------------");
                    sw.Close();
                }
            }
        }
        public void GenerateSummary(List<Staff> myStaff){
            var result =
                from staff in myStaff
                where staff.HoursWorked<10
                orderby staff.NameOfStaff ascending
                select new{staff.NameOfStaff, staff.HoursWorked};
            string path = "summary.txt";
            using(StreamWriter sw = new StreamWriter(path, true)){
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine(" ");
                foreach (var i in result){
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", i.NameOfStaff,i.HoursWorked);
                }
                sw.Close();
            }
        }
        public override string ToString()
        {
            return "Month= "+month+" Year = "+year;
        }
    }
}