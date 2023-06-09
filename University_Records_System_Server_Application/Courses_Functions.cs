using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Courses_Functions : Functionality_Operators
    {
        Authentification_Functions authentification_functions = new Authentification_Functions();


        public async Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_deletion_result = "Value deletion failed";

            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                MySqlCommand Command = new MySqlCommand("DELETE FROM departments_courses WHERE course_ID = @course_ID;", connection);

                try
                {
                    Command.Parameters.AddWithValue("course_ID", value);
                    await Command.ExecuteNonQueryAsync();

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

            return value_deletion_result;
        }

        public async Task<string> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_insertion_result = "Value insertion failed";


            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                Course course = Newtonsoft.Json.JsonConvert.DeserializeObject<Course>(value);

                if (course != null)
                {
                    MySqlCommand Command = new MySqlCommand("INSERT INTO departments_courses VALUES(@course_ID, @course_Department, @postgraduate, @location, @duration);", connection);
                    try
                    {
                        Command.Parameters.AddWithValue("course_ID", course.course_ID);
                        Command.Parameters.AddWithValue("course_Department", course.course_Department);
                        Command.Parameters.AddWithValue("postgraduate", course.postgraduate);
                        Command.Parameters.AddWithValue("location", course.location);
                        Command.Parameters.AddWithValue("duration", course.duration);

                        await Command.ExecuteNonQueryAsync();

                        value_insertion_result = "Value inserted";
                    }
                    catch (Exception E)
                    {
                        if (E.Message.Contains("Duplicate entry") == true)
                        {
                            value_insertion_result = "Course already exists";
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

                MySqlCommand Command = new MySqlCommand("SELECT * FROM departments_courses;", connection);

                try
                {

                    MySqlDataReader Reader = await Command.ExecuteReaderAsync();

                    try
                    {
                        Courses courses = new Courses();

                        while (await Reader.ReadAsync() == true)
                        {
                            Course course = new Course();

                            course.course_ID = (string)Reader["course_ID"];
                            course.course_Department = (string)Reader["course_Department"];
                            course.postgraduate = (bool)Reader["postgraduate"];
                            course.location = (string)Reader["location"];
                            course.duration = (int)Reader["duration"];

                         
                            if (courses != null)
                            {
                                courses.courses.Add(course);
                            }
                        }

                        values_selection_result = Newtonsoft.Json.JsonConvert.SerializeObject(courses);
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
                Course course = Newtonsoft.Json.JsonConvert.DeserializeObject<Course>(value);


                if (course != null)
                {

                    MySqlCommand Command = new MySqlCommand("UPDATE departments_courses SET course_Department = @course_Department, postgraduate = @postgraduate, location = @location, duration = @duration WHERE course_ID = @course_ID;", connection);

                    try
                    {
                        Command.Parameters.AddWithValue("course_ID", course.course_ID);
                        Command.Parameters.AddWithValue("course_Department", course.course_Department);
                        Command.Parameters.AddWithValue("postgraduate", course.postgraduate);
                        Command.Parameters.AddWithValue("location", course.location);
                        Command.Parameters.AddWithValue("duration", course.duration);

                        await Command.ExecuteNonQueryAsync();

                        value_modification_result = "Value modification successful";
                    }
                    catch (Exception E)
                    {

                        if (E.Message.Contains("Cannot add or update a child row: a foreign key constraint fails") == true)
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


            return value_deletion_result;
        }
    }
}
