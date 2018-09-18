using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using KaibaReduxAPI.Models;

namespace KaibaReduxAPI.Controllers
{
    public class DbAccessManagement
    // This class contains all the database access methods
    // Notice how the all methods of a certain type, for example the create methods, are very similar 
    // In fact they differ only in the SQL command string and the object taken or returned
    {
        // This is the connection string that points to the database. It is a constant so we add the readonly keyword
        // To properly run the connection string needs the server name of your SQL server instance.
        // Replace "DESKTOP-PPEIFCP" with your server's name to properly configure the connection 
        private static readonly string CONNECTION_STRING = "Data Source=DESKTOP-PPEIFCP;Initial Catalog=kaibaredux;Trusted_Connection=yes;";

        // This is the SQL connection object, which is used to execute operations on the DB
        private SqlConnection connection;



        public List<Menu> getMenus()
        // returns a list containing menu objects, which contain the information about each menu
        {
            // Declare a string list to hold the data we get from the DB
            // Note how you must declare the list's data type: <string>
            List<Menu> results = new List<Menu>();

            // Use a try catch here because it's very likely that the connection could fail and throw an error
            try
            {
                // open the connection
                OpenDb();

                // Define the SQL command statement
                // Web simply want to retrieve all the menus ordered by the position field
                string commandString = "SELECT * FROM t_menu " +
                                        "ORDER BY menuPosition";

                // Create the SQL command object, give it the command string and the connection object
                SqlCommand command = new SqlCommand(commandString, connection);

                // Execute the command, since this is a select use SqlCommand.ExecuteReader()
                // It will return a SQLDataReader object, which we assign to the variable "dataReader"
                SqlDataReader dataReader = command.ExecuteReader();

                // A DataReader allows you to read one row at a time
                // You can then call SqlDataReader.Read(), which will allow you to access the next row
                // it returns true as long as there is another row to access
                // it will return false when there are no further rows to access

                // By placing the SqlDataReader.Read() call inside a while, we can keep reading the row data until there are no further rows
                while (dataReader.Read())
                {
                    // New Menu Object to store data
                    Menu menu = new Menu();

                    // Get each column from each row
                    // The ToString() method ensures that we recieve a string 
                    menu.Id = (int) dataReader["menuID"];
                    menu.Name = dataReader["menuName"].ToString();
                    menu.Description = dataReader["menuDescription"].ToString();
                    menu.Position = (double) dataReader["menuPosition"];

                    // add that menu to the list
                    results.Add(menu);
                }

            }
            catch (Exception ex)
            // If there is an Exception (aka an error) then the catch block is executed
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);

                // If there was an error we still need to return something
                // return an empty list
                results = new List<Menu>();
            }
            finally
            {
                // whether there was an error or not, we need to close the connection
                // that's what the finally block is for
                CloseDb();
            }

            // lastly return the result
            // it's good practice to always have only a single return statement at the end of the method
            return results;
        }

        public Menu getMenu(int id)
        // takes a menu id and returns a corresponding menu object that contains it's sections, which contain items, which contain pricelines
        // if that menu is not found, returns null
        {
            // Declare return variable
            Menu result = new Menu();

            // Use a try catch here because it's very likely that the connection could fail and throw an error
            try
            {
                // open the connection
                OpenDb();

                // Define the SQL command statement
                // Web simply want to retrieve a specific menu
                string commandString = "SELECT * FROM t_menu " +
                                        "WHERE menuID = " + id;

                // Create the SQL command object, give it the command string and the connection object
                SqlCommand command = new SqlCommand(commandString, connection);

                // Execute the command, since this is a select use SqlCommand.ExecuteReader()
                // It will return a SQLDataReader object, which we assign to the variable "dataReader"
                SqlDataReader dataReader = command.ExecuteReader();

                // Since we are retriving a single row, we can use an if statement
                if (dataReader.Read())
                {
                    // There was a row returned, so we can get each column data from each row
                    result.Id = (int)dataReader["menuID"];
                    result.Name = dataReader["menuName"].ToString();
                    result.Description = dataReader["menuDescription"].ToString();
                    result.Position = (double)dataReader["menuPosition"];

                    // Close the DataReader
                    dataReader.Close();

                    // Now we need to get the sections in this menu
                    result.SectionList = GetSectionsInMenu(result.Id);

                }
                else
                {
                    // no row was returned, so the menu was not found
                    // in that case we return null, to signify that nothing was found
                    result = null;
                }

            }
            catch (Exception ex)
            // If there is an Exception (aka an error) then the catch block is executed
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);

                // If there was an error we still need to return something
                // return an empty menu, with the name Database ERROR
                result = new Menu();
                result.Name = "Database ERROR";
            }
            finally
            {
                // whether there was an error or not, we need to close the connection
                // that's what the finally block is for
                CloseDb();
            }

            // lastly return the result
            // it's good practice to always have only a single return statement at the end of the method
            return result;
        }

        public Section GetSection(int id)
        // takes a section ID and returns a section object 
        // returns null if not found
        {
            // the list to hold results from the database and eventually return
            Section result = new Section();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_section " +
                                        "WHERE sectionID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                if (dataReader.Read())
                {
                    // new section object to hold data
                    Section sect = new Section();

                    // get data from the dataReader
                    sect.Id = (int)dataReader["sectionID"];
                    sect.Name = dataReader["sectionName"].ToString();
                    sect.Description = dataReader["sectionDescription"].ToString();
                    sect.Position = (double)dataReader["sectionPosition"];
                    sect.PicturePath = dataReader["sectionPicturePath"].ToString();
                    sect.MenuID = (int)dataReader["menuID"];

                    // close the DataReader
                    dataReader.Close();

                    // get this section's items
                    sect.ItemList = GetItemsInSection(sect.Id);

                    // assign section object to the result to be returned
                    result = sect;
                }
                else
                {
                    // no row was returned, so the section was not found
                    // in that case we return null, to signify that nothing was found
                    result = null;
                }

            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // return a section with the name Database ERROR
                result = new Section();
                result.Name = "Database ERROR";
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return result;
        }

        public Item GetItem(int id)
        // takes an item ID and returns an item object 
        // returns null if not found
        {
            // the list to hold results from the database and eventually return
            Item result = new Item();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_item " +
                                        "WHERE itemID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                if (dataReader.Read())
                {
                    // new section object to hold data
                    Item item = new Item();

                    // get data from the dataReader
                    item.Id = (int)dataReader["itemID"];
                    item.Name = dataReader["itemName"].ToString();
                    item.Description = dataReader["itemDescription"].ToString();
                    item.Position = (double)dataReader["itemPosition"];
                    item.PicturePath = dataReader["itemPicturePath"].ToString();
                    item.SectionID = (int)dataReader["sectionID"];

                    // close the DataReader
                    dataReader.Close();

                    // get this item's pricelines
                    item.PriceLineList = getPricelinesForItem(item.Id);

                    // assign item object to the result to be returned
                    result = item;
                }
                else
                {
                    // no row was returned, so the item was not found
                    // in that case we return null, to signify that nothing was found
                    result = null;
                }

            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // return an item with the name Database ERROR
                result = new Item();
                result.Name = "Database ERROR";
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return result;
        }

        public Priceline GetPriceline(int id)
        // takes a pricelineID and returns a Priceline object 
        // returns null if not found
        {
            // the list to hold results from the database and eventually return
            Priceline result = new Priceline();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_priceline " +
                                        "WHERE pricelineID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                if (dataReader.Read())
                {
                    // new section object to hold data
                    Priceline price = new Priceline();

                    // get data from the dataReader
                    price.Id = (int)dataReader["pricelineID"];
                    price.Description = dataReader["pricelineDescription"].ToString();
                    price.Price = (decimal)dataReader["pricelinePrice"];
                    price.Position = (double)dataReader["pricelinePosition"];
                    price.ItemID = (int)dataReader["itemID"];

                    // close the DataReader
                    dataReader.Close();

                    // assign result
                    result = price;
                }
                else
                {
                    // wasn't found, return null
                    result = null;
                }

            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // return a priceline with the description Database ERROR
                result = new Priceline();
                result.Description = "Database ERROR";
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return result;
        }

        public List<Section> GetSectionsInMenu(int id)
        // takes a menuID and returns a list containing all the sections in that menu
        // each section will in turn contain it's corresponding items
        // in order to get this list of it's items, it calls getItemsInSection()
        {
            // the list to hold results from the database and eventually return
            List<Section> results = new List<Section>();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_section " +
                                        "WHERE menuID = " + id + " " +
                                        "ORDER BY sectionPosition";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                while (dataReader.Read())
                {
                    // new section object to hold data
                    Section sect = new Section();

                    // get data from the dataReader
                    sect.Id = (int) dataReader["sectionID"];
                    sect.Name = dataReader["sectionName"].ToString();
                    sect.Description = dataReader["sectionDescription"].ToString();
                    sect.Position = (double) dataReader["sectionPosition"];
                    sect.PicturePath = dataReader["sectionPicturePath"].ToString();
                    sect.MenuID = (int)dataReader["menuID"];

                    // put object in list
                    results.Add(sect);
                }
                // close the DataReader
                dataReader.Close();

                // use a foreach loop, to call getItemsInSection for each object
                foreach (Section s in results)
                {
                    s.ItemList = GetItemsInSection(s.Id);
                }

            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set results to be an empty list
                results = new List<Section>();
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return results;

        }

        private List<Item> GetItemsInSection(int id)
        // takes a sectionID and returns a list of all the items in that section
        // each item will contain it's own price lines
        {
            // the list to hold results from the database and eventually return
            List<Item> results = new List<Item>();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_item " +
                                        "WHERE sectionID = " + id + " " +
                                        "ORDER BY itemPosition";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                while (dataReader.Read())
                {
                    // new Item object to hold data
                    Item item = new Item();

                    // get data from dataReader
                    item.Id = (int)dataReader["itemID"];
                    item.Name = dataReader["itemName"].ToString();
                    item.Description = dataReader["itemDescription"].ToString();
                    item.Position = (double)dataReader["itemPosition"];
                    item.PicturePath = dataReader["itemPicturePath"].ToString();
                    item.SectionID = (int)dataReader["sectionID"];

                    // put Item in list
                    results.Add(item);
                }
                // close the DataReader
                dataReader.Close();

                // use a foreach to call getPricelinesForItem() on each item
                foreach (Item i in results)
                {
                    i.PriceLineList = getPricelinesForItem(i.Id);
                }
                
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // assign empty list to results
                results = new List<Item>();
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return results;
        }

        private List<Priceline> getPricelinesForItem(int id)
        // takes an item ID and returns a list of all pricelines associated with that item
        {
            // the list to hold results from the database and eventually return
            List<Priceline> results = new List<Priceline>();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "SELECT * FROM t_priceLine " +
                                        "WHERE itemID = " + id + " " +
                                        "ORDER BY pricelinePosition";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                while (dataReader.Read())
                {
                    // new Priceline object to hold data
                    Priceline price = new Priceline();

                    // get data from dataReader
                    price.Id = (int)dataReader["pricelineID"];
                    price.Description = dataReader["pricelineDescription"].ToString();
                    price.Price = (decimal)dataReader["pricelinePrice"];
                    price.Position = (double)dataReader["pricelinePosition"];
                    price.ItemID = (int)dataReader["itemID"];

                    // put Priceline in list
                    results.Add(price);
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // assign empty list to results
                results = new List<Priceline>();
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return results;
        }

        public bool InsertMenu (Menu menu)
            // takes a menu object and creates a coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // Note how this uses dynamic SQL and drops values directly into the command string
                // However, this leaves the application vulnerable to SQL injection
                // using prepared statements (aka parameterized) would be a better solution
                string commandString = "INSERT INTO t_menu (menuName, menuDescription, menuPosition) " +
                                        "VALUES ('" + menu.Name + "', '" + menu.Description + "', " + menu.Position + ")";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool InsertSection(Section section)
        // takes a Section object and creates a coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // Note how this uses dynamic SQL and drops values directly into the command string
                // However, this leaves the application vulnerable to SQL injection
                // using prepared statements (aka parameterized) would be a better solution
                string commandString = "INSERT INTO t_section (sectionName, sectionDescription, sectionPosition, sectionPicturePath, menuID) " +
                                        "VALUES ('" + section.Name + "', '" + section.Description + "', " + section.Position + ", '" + section.PicturePath + "', " + section.MenuID + ")";

                System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAA: " + commandString);
                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool InsertItem(Item item)
        // takes a Item object and creates a coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // Note how this uses dynamic SQL and drops values directly into the command string
                // However, this leaves the application vulnerable to SQL injection
                // using prepared statements (aka parameterized) would be a better solution
                string commandString = "INSERT INTO t_item (itemName, itemDescription, itemPosition, itemPicturePath, sectionID) " +
                                        "VALUES ('" + item.Name + "', '" + item.Description + "', " + item.Position + ", '" + item.PicturePath + "', " + item.SectionID + ")";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool InsertPriceline(Priceline price)
        // takes a Priceline object and creates a coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // Note how this uses dynamic SQL and drops values directly into the command string
                // However, this leaves the application vulnerable to SQL injection
                // using prepared statements (aka parameterized) would be a better solution
                string commandString = "INSERT INTO t_priceline (pricelineDescription, pricelinePrice, pricelinePosition, itemID) " +
                                        "VALUES ('" + price.Description + "', " + price.Price + ", " + price.Position + ", " + price.ItemID + ")";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool UpdateMenu(Menu menu)
        // takes a menu object and updates the coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "UPDATE t_menu SET " +
                                        "menuName = '" + menu.Name + "', " +
                                        "menuDescription = '" + menu.Description + "', " +
                                        "menuPosition = " + menu.Position + " " +
                                        "WHERE menuID = " + menu.Id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() 
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool UpdateSection(Section section)
        // takes a Section object and updates the coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "UPDATE t_section SET " +
                                        "sectionName = '" + section.Name + "', " +
                                        "sectionDescription = '" + section.Description + "', " +
                                        "sectionPosition = " + section.Position + ", " +
                                        "sectionPicturePath = '" + section.PicturePath + "', " +
                                        "menuID = " + section.MenuID + " " +
                                        "WHERE sectionID = " + section.Id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() 
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool UpdateItem(Item item)
        // takes an Item object and updates the coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "UPDATE t_item SET " +
                                        "itemName = '" + item.Name + "', " +
                                        "itemDescription = '" + item.Description + "', " +
                                        "itemPosition = " + item.Position + ", " +
                                        "itemPicturePath = '" + item.PicturePath + "', " +
                                        "sectionID = " + item.SectionID + " " +
                                        "WHERE itemID = " + item.Id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() 
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool UpdatePriceline(Priceline price)
        // takes an Priceline object and updates the coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "UPDATE t_priceline SET " +
                                        "pricelineDescription = '" + price.Description + "', " +
                                        "pricelinePrice = " + price.Price + ", " +
                                        "pricelinePosition = " + price.Position + ", " +
                                        "itemID = " + price.ItemID + " " +
                                        "WHERE pricelineID = " + price.Id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() 
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool DeleteMenu(int id)
        // takes a menu id and deletes the coresponding database entry for it
        // will not work if the menu still has sections in it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // if this menu has any sections in it, this will fail
                string commandString = "DELETE FROM t_menu " +
                                        "WHERE menuID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool DeleteSection(int id)
        // takes a section id and deletes the coresponding database entry for it
        // will not work if the section still has items in it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // if this section has any items in it, this will fail
                string commandString = "DELETE FROM t_section " +
                                        "WHERE sectionID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool DeleteItem(int id)
        // takes an Item id and deletes the coresponding database entry for it
        // will not work if the item still has pricelines assigned to it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                // if this item has any pricelines in it, this will fail
                string commandString = "DELETE FROM t_item " +
                                        "WHERE itemID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public bool DeletePriceline(int id)
        // takes a priceline id and deletes the coresponding database entry for it
        {
            // this boolean will represent whether the operation was successful or not
            bool result = false;

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString = "DELETE FROM t_priceline " +
                                        "WHERE pricelineID = " + id;

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command using command.executeNonQuery() because this will not return a DataReader
                // command.executeNonQuery() returns the number of rows affected, so assign that to a variable
                int rowsAffected = command.ExecuteNonQuery();

                // if one row was affected, then we were successful
                if (rowsAffected == 1)
                {
                    result = true;
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // set result to false
                result = false;
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the result
            return result;
        }

        public List<Object> DbOperationTemplate(string input)
            // this class is merely a template for each method in this class
        {
            // the list to hold results from the database and eventually return
            List<Object> results = new List<object>();

            // try block to contain DB access statements
            try
            {
                // open the connection
                OpenDb();

                // define SQL command string
                string commandString =  "SELECT * FROM TABLE_NAME " +
                                        "WHERE TABLE_ID = SOME_VALUE";

                // create sql command object
                SqlCommand command = new SqlCommand(commandString, connection);

                // excecute command and assign results to a dataReader
                SqlDataReader dataReader = command.ExecuteReader();

                // while loop to get all row data
                while (dataReader.Read())
                {
                    // new object to hold data
                    Object obj = new Object();

                    // get data from dataReader
                    // obj.ATTRIBUTE_NAME = dataReader["COLUMN_NAME"].ToString();
                    // obj.ATTRIBUTE_NAME = (int) dataReader["COLUMN_NAME"];

                    // put object in list
                    results.Add(obj);
                }
            }
            // catch block to handle any errors
            catch (Exception ex)
            {
                // Write the error to the console
                // The "DB-DEBUG:" is just there to make finding that message in the console easier
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.StackTrace);

                // assign empty list to results
                results = new List<object>();
            }
            // finally block in which we close the connection, whether or not there was an error
            finally
            {
                CloseDb();
            }

            // lastly return the results
            return results;
        }


        private void OpenDb()
        // Opens the database connection. This must be done before every db operation.
        {
            if (connection == null)
            {
                connection = new SqlConnection(CONNECTION_STRING);
                connection.Open();
            }
        }

        private void CloseDb()
        // Closes the database connection. This should be done after every database operation, whether it suceeded or not
        // This is a general good programming practice, as it frees up system resources (makes sure you're not opening a new connection every time, but not closing them)
        {
            // check if the connection is already null
            // If the connection was null and we tried to close it, we would get a NullPointerException
            if (connection != null)
            {
                // if it isn't null, then we need to close it
                connection.Close();

                // and set it to null
                connection = null;
            }
        }


        public static bool DBTest()
        //a function to test whether the connection can be opened and closed without an error
        {
            // Declare a connection object
            SqlConnection cnn;

            // Instantiate connection object
            // Give it the connection string constant
            cnn = new SqlConnection(CONNECTION_STRING);

            // Try opening and closing the connection
            try
            {
                cnn.Open();
                cnn.Close();
                // If connection opened and closed without errors, output a confirmation to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: Connection worked");
                return true;
            }
            catch (Exception ex)
            {
                // If there was an error, output it to the console
                System.Diagnostics.Debug.WriteLine("DB-DEBUG: " + ex.Message);
            }
            return false;
        }

        public static string DBTest2()
        // Try outputting information from the db to the page
        {
            SqlConnection cnn;
            cnn = new SqlConnection(CONNECTION_STRING);
            try
            {
                string result = " ";
                cnn.Open();

                string sqlString = "SELECT * FROM t_item i, t_priceline p WHERE i.itemID = p.itemID";

                SqlCommand myCommand = new SqlCommand(sqlString, cnn);

                SqlDataReader myReader = myCommand.ExecuteReader();
                bool firstTime = true;
                while (myReader.Read())
                {
                    if (!firstTime)
                    {
                        result += ", ";
                    }
                    firstTime = false;

                    result += "[";
                    result += (myReader["itemName"].ToString()) + ",";
                    result += (myReader["itemDescription"].ToString()) + ",";
                    result += (myReader["itemPicturePath"].ToString()) + ",";
                    result += (myReader["pricelinePrice"].ToString()) + "";
                    result += "]";
                }

                result += " ";

                cnn.Close();
                System.Diagnostics.Debug.WriteLine("Connection worked");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public static string DBTest3()
        {
            SqlConnection cnn;
            cnn = new SqlConnection(CONNECTION_STRING);
            try
            {
                string result = " ";
                cnn.Open();

                string sqlString = "SELECT * FROM t_item i, t_priceline p WHERE i.itemID = p.itemID";

                SqlCommand myCommand = new SqlCommand(sqlString, cnn);

                SqlDataReader myReader = myCommand.ExecuteReader();
                bool firstTime = true;
                while (myReader.Read())
                {
                    if (!firstTime)
                    {
                        result += ", ";
                    }
                    firstTime = false;

                    result += "\"item\": { ";
                    result += "\"name\" : \"" + (myReader["itemName"].ToString()) + "\",";
                    result += "\"description\" : \"" + (myReader["itemDescription"].ToString()) + "\",";
                    result += "\"picturePath\" : \"" + (myReader["itemPicturePath"].ToString()) + "\",";
                    result += "\"price\" : \"" + (myReader["pricelinePrice"].ToString()) + "";
                    result += "}";

                }

                result += " ";

                cnn.Close();
                System.Diagnostics.Debug.WriteLine("Connection worked");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ex.Message;
            }
        }
    }
}
