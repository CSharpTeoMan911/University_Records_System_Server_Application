using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Student_Functions : Functionality_Operators
    {
        Authentification_Functions authentification_functions = new Authentification_Functions();


        public async Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_deletion_result = "Value deletion failed";

            if(await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                MySqlCommand Command = new MySqlCommand("DELETE FROM student_data WHERE student_ID = @student_ID;", connection);

                try
                {
                    Command.Parameters.AddWithValue("student_ID", value);
                    await Command.ExecuteNonQueryAsync();

                    value_deletion_result = "Value deletion successful";
                }
                catch
                {
                    
                }
                finally
                {
                    if(Command != null)
                    {
                        await Command.DisposeAsync();
                    }
                }
            }

            return value_deletion_result;
        }

        public async Task<string> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_insertion_result = "Value insertion failed";


            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                Student student = Newtonsoft.Json.JsonConvert.DeserializeObject<Student>(value);

                if (student != null)
                {
                    MySqlCommand Command = new MySqlCommand("INSERT INTO Student_Data VALUES(@student_ID, @full_name, @DOB, @course_ID);", connection);
                    try
                    {
                        Command.Parameters.AddWithValue("student_ID", student.student_ID);
                        Command.Parameters.AddWithValue("full_name", student.full_name);
                        Command.Parameters.AddWithValue("DOB", student.DOB);
                        Command.Parameters.AddWithValue("course_ID", student.course_ID);

                        await Command.ExecuteNonQueryAsync();

                        value_insertion_result = "Value inserted";
                    }
                    catch(Exception E)
                    {
                        if(E.Message.Contains("Duplicate entry") == true)
                        {
                            value_insertion_result = "Student already exists";
                        }
                        else if(E.Message.Contains("Cannot add or update a child row: a foreign key constraint fails") == true)
                        {
                            value_insertion_result = "Course does not exist";
                        }
                    }
                    finally
                    {
                        if (Command != null)
                        {
                            await Command.DisposeAsync();
                        }
                    }
                }
            }

            return value_insertion_result;
        }


        public async Task<string> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string values_selection_result = "Value selection failed";

            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {

                MySqlCommand Command = new MySqlCommand("SELECT * FROM student_data;", connection);

                try
                {
                  
                    MySqlDataReader Reader = await Command.ExecuteReaderAsync();

                    try
                    {
                        Students students = new Students();

                        while (await Reader.ReadAsync() == true)
                        {
                            Student student = new Student();

                            student.student_ID = (string)Reader["student_ID"];
                            student.full_name = (string)Reader["full_name"];
                            student.DOB = (DateTime)Reader["DOB"];
                            student.course_ID = (string)Reader["course_ID"];

                            if(student != null)
                            {
                                students.students.Add(student);
                            }
                        }

                        values_selection_result = Newtonsoft.Json.JsonConvert.SerializeObject(students);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (Reader != null)
                        {
                            await Reader.DisposeAsync();
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    if (Command != null)
                    {
                        await Command.DisposeAsync();
                    }
                }
            }

            return values_selection_result;
        }




        public async Task<string> Modify_Entity_Data(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_modification_result = "Value modification failed";

            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                Student student = Newtonsoft.Json.JsonConvert.DeserializeObject<Student>(value);


                if(student != null)
                {

                    MySqlCommand Command = new MySqlCommand("UPDATE student_data SET full_name = @full_name, DOB = @DOB, course_ID = @course_ID WHERE student_ID = @student_ID;", connection);

                    try
                    {
                        Command.Parameters.AddWithValue("full_name", student.full_name);
                        Command.Parameters.AddWithValue("DOB", student.DOB);
                        Command.Parameters.AddWithValue("course_ID", student.course_ID);
                        Command.Parameters.AddWithValue("student_ID", student.student_ID);

                        await Command.ExecuteNonQueryAsync();

                        value_modification_result = "Value modification successful";
                    }
                    catch(Exception E)
                    {

                        if(E.Message.Contains("Cannot add or update a child row: a foreign key constraint fails") == true)
                        {
                            value_modification_result = "Course does not exist";
                        }
                    }
                    finally
                    {
                        if (Command != null)
                        {
                            await Command.DisposeAsync();
                        }
                    }
                }
            }

            return value_modification_result;
        }


        public async Task<string> Select_Values_By_Criteria_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_deletion_result = "Value deletion failed";

            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {


                Student student = Newtonsoft.Json.JsonConvert.DeserializeObject<Student>(value);

                if(student != null)
                {
                    if(student.DOB != null && student.full_name != String.Empty && student.course_ID != String.Empty)
                    {
                        MySqlCommand Command = new MySqlCommand("SELECT * FROM student_data WHERE full_name = @full_name AND DOB = @DOB AND course_ID = @course_ID", connection);

                        try
                        {
                            Command.Parameters.AddWithValue("full_name", value);
                            Command.Parameters.AddWithValue("DOB", value);
                            Command.Parameters.AddWithValue("course_ID", value);


                            value_deletion_result = "Value deletion successful";
                        }
                        catch
                        {

                        }
                        finally
                        {
                            if (Command != null)
                            {
                                await Command.DisposeAsync();
                            }
                        }
                    }
                }

            }

            return value_deletion_result;
        }
    }
}
