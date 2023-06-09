﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Grades_Functions : Functionality_Operators
    {
        Authentification_Functions authentification_functions = new Authentification_Functions();

        public async Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_deletion_result = "Value deletion failed";

            if (await authentification_functions.Log_In_Session_Key_Validation(log_in_session_key, connection) == "Log in session key validated")
            {
                MySqlCommand Command = new MySqlCommand("DELETE FROM courses_grades WHERE grade_id = @grade_id;", connection);

                try
                {
                    Command.Parameters.AddWithValue("grade_id", value);
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
                Grade grade = Newtonsoft.Json.JsonConvert.DeserializeObject<Grade>(value);

                if (grade != null)
                {
                    MySqlCommand Command = new MySqlCommand("INSERT INTO courses_grades VALUES(@grade_id, @student_ID, @course_ID, @subject_module, @student_grade);", connection);
                    try
                    {
                        Command.Parameters.AddWithValue("grade_id", grade.grade_id);
                        Command.Parameters.AddWithValue("student_ID", grade.student_ID);
                        Command.Parameters.AddWithValue("course_ID", grade.course_ID);
                        Command.Parameters.AddWithValue("subject_module", grade.subject_module);
                        Command.Parameters.AddWithValue("student_grade", grade.student_grade);

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

                MySqlCommand Command = new MySqlCommand("SELECT * FROM courses_grades;", connection);

                try
                {

                    MySqlDataReader Reader = await Command.ExecuteReaderAsync();

                    try
                    {
                        Grades grades = new Grades();
                        
                        while (await Reader.ReadAsync() == true)
                        {

                            Grade grade = new Grade();

                            grade.grade_id = (int)Reader["grade_id"];
                            grade.student_ID = (string)Reader["student_ID"];
                            grade.course_ID = (string)Reader["course_ID"];
                            grade.subject_module = (string)Reader["subject_module"];
                            grade.student_grade = (int)Reader["student_grade"];


                            if (grade != null)
                            {
                                grades.grades.Add(grade);
                            }
                        }

                        values_selection_result = Newtonsoft.Json.JsonConvert.SerializeObject(grades);
                    }
                    catch(Exception E)
                    {
                        System.Diagnostics.Debug.WriteLine(E.Message);
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
                Grade grade = Newtonsoft.Json.JsonConvert.DeserializeObject<Grade>(value);


                if (grade != null)
                {

                    MySqlCommand Command = new MySqlCommand("UPDATE courses_grades SET student_grade = @student_grade, student_ID = @student_ID, course_ID = @course_ID, subject_module = @subject_module WHERE grade_id = @grade_id;", connection);

                    try
                    {
                        Command.Parameters.AddWithValue("grade_id", grade.grade_id);
                        Command.Parameters.AddWithValue("student_ID", grade.student_ID);
                        Command.Parameters.AddWithValue("course_ID", grade.course_ID);
                        Command.Parameters.AddWithValue("subject_module", grade.student_grade);
                        Command.Parameters.AddWithValue("student_grade", grade.student_grade);

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
