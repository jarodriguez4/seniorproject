// This a separate Javascript file that contains functions used throughout the whole website.

// This url should point to API 
const URL = "api/";

// The id of the div in which to put the elements describing all the menus
const MENUS_LIST_DIV_ID = "#menuList";

// The id of the div in which to put the elements describing the selected menu's contents
const MENU_CONTENTS_DIV_ID = "#menuContents";

// The id of the div on the editItem page that contains the pricelines
const PRICELINE_DIV_ID = "#pricelineDiv";


function getMenus() {
    // This function gets the list of menus from the API
    // It displays them as <p> elements within the div given by constant MENUS_LIST_DIV_ID
    // It sets the onclick event of each to call showMenu(#), where # is that menu's id number

    // Jquery ajax call to get the list containing the menu info
    $.ajax({                        // The $ (dollar sign) is used to access the Jquery functions, in this case an ajax call
        method: 'GET',              // This is a GET request
        url: URL + "menu",          // The URL we want is api/menu
        dataType: "json",           // The datatype we expect the server to return, the JSON will automatically be parsed and converted into a JavaScript object
        success: function (menuList) {  // This inline function will be called if the request is successful
            // Note that this function is called asynchronously at a later time, so any data you get back must be maniputed here.
            // the variable 'menuList' is now a js array that holds menu objects
            // It cannot be used outside of this function

            // Use Jquery to empty the menuList div of any elements
            $(MENUS_LIST_DIV_ID).empty();

            // loop through them
            for (let i = 0; i < menuList.length; i++) {
                let menu = menuList[i]; 

                // add line break if not the first menu
                if (i > 0) {
                    $('<br />').appendTo($(MENUS_LIST_DIV_ID));
                }

                // Using Jquery create a new <p> object with the menu's info
                // It has id property: id = menu#, where # is that menu's id number
                // It also adds an onclick event, so that when clicked it will call showMenu(#), where # is that menu's id number
                $('<button id="menu' + menu.id + '" onclick="showMenu(' + menu.id + ')"><strong> Show ' + menu.name + '</strong></button>' +
                    '<button class="edit" onclick="editMenu(' + menu.id + ')"> Edit </button>' + // adds a button to go to the menu edit page
                    '<button id="deleteMenu' + menu.id + '" class="delete" onclick="deleteMenu(' + menu.id + ')"> Delete </button>' // adds a button to delete the menu
                ).appendTo($(MENUS_LIST_DIV_ID)); // Add it to the menuList div
            }

            // add a create new menu button
            $('<br /> <button class="edit" onclick="editMenu()"> Create new Menu </button>').appendTo($(MENUS_LIST_DIV_ID));

        },
        error: function (jqXHR, status, errorThrown) {      // This function will run if there's an error
            alert("ERROR: Can't retrieve menu list " + errorThrown + " ");   // Pop up a textbox with an error message
        }
    });
    

}

function showMenu(id = 1) {
    // shows a specific menu, the first paramenter is the id, which defaults to the current menu being shown
    // The affected div is given by the MENU_CONTENTS_DIV_ID constant 
    // creates nested divs and <p> inside the specified div (also clears any elements currently in the div)

    // Jquery ajax call to get the specific menu
    $.ajax({
        method: 'GET',
        url: URL + "menu/" + id,    // The URL we want is api/menu/#
        dataType: "json",           
        success: function (menu) {  // function called if successful
            // Note that this function is called asynchronously at a later time, so any data you get back must be maniputed here.
            // the variable 'menu' is now a menu object that cannot be used outside of this function

            // Use Jquery to empty the div of any elements
            $(MENU_CONTENTS_DIV_ID).empty();
            
            // Using Jquery create a new <p> elements with the menu's info
            $('<p id="menu' + menu.id + '" ><strong>' + menu.name + '</strong></p>' +
                '<p><i> ' + menu.description + ' </i></p>'
            ).appendTo($(MENU_CONTENTS_DIV_ID)); // Add it to the specified div
            
            // add a create new section button, passes a negative id, which signifies that it is a create, the negative number is the menuId to within which the section should be created
            $('<button class="edit" onclick="editSection(-' + menu.id + ')"> Create new Section in ' + menu.name + '</button> <br /> <br />').appendTo($(MENU_CONTENTS_DIV_ID));

            // loop through the sectionList variable of the menu object
            for (let i = 0; i < menu.sectionList.length; i++) {
                let section = menu.sectionList[i];

                // display each section 
                showSection(section);
            }

            if (menu.sectionList.length > 0) {
                // add a create new section button
                $('<br /> <button class="edit" onclick="editSection(-' + menu.id + ')"> Create new Section in ' + menu.name + '</button>').appendTo($(MENU_CONTENTS_DIV_ID));
            }
            
            // change id of the current menu being shown
            currentMenuID = id;

            // check if edit buttons should be hidden
            if (editButtonsShowing == false) {
                hideEditButtons();
                console.log("hide");
            }
        },
        error: function (jqXHR, status, errorThrown) {      // This function will run if there's an error
            alert("ERROR: Can't retrieve that menu " + errorThrown + " ");   // Pop up a textbox with an error message
        }
    });
}

function showSection(section) {
    // takes a section object and adds <p> describing it to the menu contents div (given by the constant MENU_CONTENTS_DIV_ID)

    $('<p id="section' + section.id + '" ><strong>' + section.name + '</strong></p>' +
        '<p><i> ' + section.description + ' </i></p>' +
        '<button class="edit" onclick="editSection(' + section.id + ')"> Edit ' + section.name + '</button>' + // adds a button to go to the section edit page
        '<button id="deleteSection' + section.id + '" class="delete" onclick="deleteSection(' + section.id + ')"> Delete ' + section.name + '</button>' + // adds a button to delete the section
        '<br />'
    ).appendTo($(MENU_CONTENTS_DIV_ID)); // Add it to the specified div

    // add a create new item button
    $('<button class="edit" onclick="editItem(-' + section.id + ', ' + section.menuID + ')"> Create new Item in ' + section.name + '</button> <br /> <br />').appendTo($(MENU_CONTENTS_DIV_ID));

    // loop through the itemList variable of the section object
    for (let i = 0; i < section.itemList.length; i++) {
        let item = section.itemList[i];

        // display each item 
        showItem(item, section.menuID);
    }
    
}

function showItem(item, menuID = 1) {
    // takes an item object and adds <p> describing it to the menu contents div (given by the constant MENU_CONTENTS_DIV_ID)
    // second parameter is the menuID of the section that this menu is in, which defaults to 1 if not specified

    $('<p id="item' + item.id + '" >' + item.position + '. ' + item.name + '</p>' +
        '<p><i> ' + item.description + ' </i></p>' +
        '<button class="edit" onclick="editItem(' + item.id + ', ' + menuID + ')"> Edit ' + item.name + '</button>' + // adds a button to go to the item edit page
        '<button id="deleteItem' + item.id + '" class="delete" onclick="deleteItem(' + item.id + ')"> Delete ' + item.name + '</button>' // adds a button to delete the item
    ).appendTo($(MENU_CONTENTS_DIV_ID)); // Add it to the specified div

    // check if there's only one priceline item with no description
    if (item.priceLineList.length == 1 && item.priceLineList[0].description == "") {
        // append the price to the end of the item name <p>
        $('#item' + item.id).append($('<span> - $' + item.priceLineList[0].price + '</span>'));
    }
    else {
        // otherwise loop through the priceLineList variable of the item object
        for (let i = 0; i < item.priceLineList.length; i++) {
            let priceline = item.priceLineList[i];

            // display each item 
            showPriceline(priceline);
        }
    }

    // add line break
    $('<br />').appendTo($(MENU_CONTENTS_DIV_ID));
}

function showPriceline(priceline) {
    // takes an priceline object and adds <p> describing it to the menu contents div (given by the constant MENU_CONTENTS_DIV_ID)

    $('<p id="priceline' + priceline.id + '" >' + priceline.description + ' - $' + priceline.price + '</p>'
    ).appendTo($(MENU_CONTENTS_DIV_ID)); // Add it to the specified div
}

function showEditButtons() {
    // shows all edit buttons (which start hidden)
    $("#loginButton").text("Logout");
    $("#loginDesc").text("Press to hide edit buttons");
    $(".edit").show();
    $(".delete").show();
}

function hideEditButtons() {
    // shows all edit buttons (which start hidden)
    $("#loginButton").text("Login");
    $("#loginDesc").text("Press to show edit buttons");
    $(".edit").hide();
    $(".delete").hide();
}

function getQueryParam(param) {
    // searches the url parameters for a specified parameter, which it returns if it exists
    // returns null if the parameter is not found
    let result = null;
    location.search.substr(1)
        .split("&")
        .some(function (item) { // returns first occurence and stops
            return item.split("=")[0] == param && (result = item.split("=")[1])
        })
    return result
}

function editMenu(id = null) {
    // redirects to the Menu Edit/Create page, passes an id as a url parameter if one is passed

    let param = "";
    if (id != null) {
        param = "?id=" + id;
    }
    window.location = "editMenu.html" + param;
}

function loadMenu(id) {
    // takes a menu id and loads it's info into the input elements (id, name, description, and position)
    // Jquery ajax call to get the specific menu
    $.ajax({
        method: 'GET',
        url: URL + "menu/" + id,    // The URL we want is api/menu/#
        dataType: "json",
        success: function (menu) {  // function called if successful
            $('#id').val(menu.id);
            $('#name').val(menu.name);
            $('#description').val(menu.description);
            $('#position').val(menu.position);
        },
        error: function (jqXHR, status, errorThrown) {      // This function will run if there's an error
            alert("ERROR: Can't retrieve that menu " + errorThrown + " ");   // Pop up a textbox with an error message
        }
    });
}

function submitMenu(create) {
    // takes the values of the menu input elements (id, name, description, and position)
    // and passes them to the api using create if true is passed and using edit if false is passed

    // data validation: check if position is a number
    let errorString = isPositiveNumber($('#position').val(),"Position");

    if (errorString == "") {

        // create menu object by getting value attribute of the input elements
        let menu = {
            "id": parseInt($('#id').val()),
            "name": $('#name').val(),
            "description": $('#description').val(),
            "position": parseFloat($('#position').val())
        };

        // request method is either POST (create) or PUT (edit)
        let methodString, operationString;
        if (create) {
            methodString = "POST";
            operationString = "created";
        }
        else {
            methodString = "PUT";
            operationString = "updated";
        }

        // ajax call
        $.ajax({
            method: methodString,               // either POST or PUT
            accepts: 'application/json',
            url: URL + "menu",                  // url is api/menu
            contentType: 'application/json',    // type of data being sent
            data: JSON.stringify(menu),         // actual data being sent, use JSON library to convert the menu object to JSON
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error: Menu could not be ' + operationString);
            },
            success: function (result) {
                alert('Menu successfully ' + operationString);

                var redirectParam = ""
                // if this is an edit redirect to that menu display
                if (!create) {
                    redirectParam = "?id=" + menu.id;
                }
                window.location = "index.html" + redirectParam;

            }
        });
    }
    else {
        // if there was an error display it
        alert(errorString);
    }
}

function deleteMenu(id) {
    // called by a delete button
    // first call changes the button's text to "Confirm Delete?"
    // second call sends an ajax request to delete that entry, then gets the menus again
    
    // the confirm delete text
    let confirmDeleteText = "Confirm Delete?";

    // id of current button
    let buttonID = "#deleteMenu" + id;

    if ($(buttonID).text() != confirmDeleteText) {
        // first call of deleteMenu() changes the text of the calling button
        $(buttonID).text(confirmDeleteText)
        console.log($(this).text());
    }
    else {
        // second call sends the ajax delete request

        // JSON to be sent, only requires the id
        let jsonString = '{ "id": ' + id +' }';

        $.ajax({
            method: "DELETE",                     // method is DELETE
            accepts: 'application/json',
            url: URL + "menu",                  // url is api/menu
            contentType: 'application/json',    // type of data being sent
            data: jsonString,                   // actual data being sent
            success: function (result) {
                // Change button text
                $(buttonID).text("Successfully Deleted");

                // Remove this button's onclick event
                $(buttonID).prop('onclick', null).off('click');

                // refresh menus
                getMenus();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // Change button text to show delete failure
                $(buttonID).text("Delete Failed. There are still sections in this menu");
            }
        });
    }
}

function editSection(id = null) {
    // redirects to the Section Edit/Create page, passes an id as a url parameter if one is passed

    let param = "";
    if (id != null) {
        param = "?id=" + id;
    }
    window.location = "editSection.html" + param;
}

function loadSection(id) {
    // takes a section id and loads it's info into the input elements (id, name, description, and position)

    // Jquery ajax call to get the specific section
    $.ajax({
        method: 'GET',
        url: URL + "section/" + id,    // The URL we want is api/section/#
        dataType: "json",
        success: function (section) {  // function called if successful

            // load data into the input elements
            $('#id').val(section.id);
            $('#name').val(section.name);
            $('#description').val(section.description);
            $('#position').val(section.position);
            $('#menuID').val(section.menuID);
            $('#picturePath').val(section.picturePath);

            // change the back link to go to this section's menu
            $("#menuDisplayLink").attr("href", "index.html?id=" + section.menuID);
        },
        error: function (jqXHR, status, errorThrown) {      // This function will run if there's an error
            alert("ERROR: Can't retrieve that section " + errorThrown + " ");   // Pop up a textbox with an error message
        }
    });
}

function submitSection(create) {
    // takes the values of the section input elements (id, name, description, and position)
    // and passes them to the api using create if true is passed and using edit if false is passed


    // data validation check if position is a number
    let errorString = isPositiveNumber($('#position').val(), "Position");

    if (errorString == "") {


        // create section object by getting value attribute of the input elements
        let section = {
            "id": parseInt($('#id').val()),
            "name": $('#name').val(),
            "description": $('#description').val(),
            "position": parseFloat($('#position').val()),
            "menuID": $('#menuID').val(),
            "picturePath": $('#picturePath').val()
        };

        // request method is either POST (create) or PUT (edit)
        let methodString, operationString;
        if (create) {
            methodString = "POST";
            operationString = "created";
        }
        else {
            methodString = "PUT";
            operationString = "updated";
        }
        //console.log(JSON.stringify(section));

        // ajax call
        $.ajax({
            method: methodString,               // either POST or PUT
            accepts: 'application/json',
            url: URL + "section",                  // url is api/section
            contentType: 'application/json',    // type of data being sent
            data: JSON.stringify(section),         // actual data being sent, use JSON library to convert the section object to JSON
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error: Section could not be ' + operationString);
            },
            success: function (result) {
                alert('Section successfully ' + operationString);
                window.location = "index.html?id=" + section.menuID;

            }
        });
    }
    else {
        // display error
        alert(errorString);
    }
}

function deleteSection(id) {
    // called by a delete button
    // first call changes the button's text to "Confirm Delete?"
    // second call sends an ajax request to delete that entry, then gets the sections again

    // the confirm delete text
    let confirmDeleteText = "Confirm Delete, including all items in this section?";

    // id of current button
    let buttonID = "#deleteSection" + id;

    if ($(buttonID).text() != confirmDeleteText) {
        // first call changes the text of the calling button
        $(buttonID).text(confirmDeleteText)
    }
    else {
        // second call sends the ajax delete request

        // JSON to be sent, only requires the id
        let jsonString = '{ "id": ' + id + ' }';

        $.ajax({
            method: "DELETE",                     // method is DELETE
            accepts: 'application/json',
            url: URL + "section",                  // url is api/section
            contentType: 'application/json',    // type of data being sent
            data: jsonString,                   // actual data being sent
            success: function (result) {
                // Change button text
                $(buttonID).text("Successfully Deleted");

                // Remove this button's onclick event
                $(buttonID).prop('onclick', null).off('click');

                // refresh current menu
                showMenu(currentMenuID);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // Change button text to show delete failure
                $(buttonID).text("Delete Failed");
            }
        });
    }
}

function editItem(itemID, menuID) {
    // redirects to the Item Edit/Create page,
    // first parameter is either a positive item or a negative section id
    // (a negative number signifies that this is a create operation)
    // second parameter is a menuID

    let param = "?id=" + itemID + "&menuID=" + menuID;
    window.location = "editItem.html" + param;
}

function loadItem(id) {
    // takes an item id and loads it's info into the input elements (id, name, description, position, etc)
    // Also adds elements describing its associated pricelines to the pricelineDiv


    // Jquery ajax call to get the specific item
    $.ajax({
        method: 'GET',
        url: URL + "item/" + id,    // The URL we want is api/item/#
        dataType: "json",
        success: function (item) {  // function called if successful
            $('#id').val(item.id);
            $('#name').val(item.name);
            $('#description').val(item.description);
            $('#position').val(item.position);
            $('#sectionID').val(item.sectionID);
            $('#picturePath').val(item.picturePath);

            // empty elements from the PRICELINE_DIV_ID 
            $(PRICELINE_DIV_ID).empty();

            // add a button to add another priceline form
            $('<button class="edit" id="newPricelineButton"> Add new priceline </button> <br/> <br/> ').appendTo($(PRICELINE_DIV_ID)); 
            // add onclick event to add the form and hide this button
            $('#newPricelineButton').click(function() {
                listPriceline(null, item.id);
                $(this).hide();
            });
            
            // loop through pricelines
            for (let i = 0; i < item.priceLineList.length; i++) {
                let priceline = item.priceLineList[i];

                listPriceline(priceline);
            }
        },
        error: function (jqXHR, status, errorThrown) {      // This function will run if there's an error
            alert("ERROR: Can't retrieve this item " + errorThrown + " ");   // Pop up a textbox with an error message
        }
    });
}

function listPriceline(priceline, itemID) {
    // takes a priceline object and adds input fields with it's contents to the PRICELINE_DIV_ID
    // and also adds submit changes and delete priceline buttons
    // if the priceline object is null, it substitutes an empty priceline object with id = 0 (for a create operation, where id doesn't matter)
    // the second parameter is an itemID, given when this is a create

    //console.log(priceline);

    // if it's null then this is a create
    if (priceline == null) {
        var create = true;
        priceline = {
            "id": 0,
            "description": "",
            "price": "",
            "position": "",
            "itemID": itemID
        };
        console.log(priceline);
    }
    else {
        var create = false;
    }

    // create the html string of input elements
    var htmlString = "";

    if (create) {
        htmlString += '<span>New Priceline Submission: </span><br>';
    }

    htmlString += 
        '<span>Priceline Description </span><br>' +
        '<input id="pricelineDescription' + priceline.id + '" type = "text" value = "' + priceline.description + '"> <br />' +
        '<span>Priceline Price ($) </span><br>' +
        '<input id="pricelinePrice' + priceline.id + '" type = "text" value = "' + priceline.price + '"> <br />' +
        '<span>Priceline Position </span><br>' +
        '<input id="pricelinePosition' + priceline.id + '" type = "text" value = "' + priceline.position + '">' +
        '<input id="pricelineItemID' + priceline.id + '" type = "hidden" value = "' + priceline.itemID + '">' +
        '<input id="pricelineID' + priceline.id + '" type = "hidden" value = "' + priceline.id + '"> <br />';

    // only want a submit button if this is a create
    if (create) {
        htmlString += '<button class="edit" onclick="submitPriceline(' + priceline.id + ', true)"> Submit New Priceline </button>' // submit button;
    }
    else {
        htmlString += 
            '<button class="edit" onclick="submitPriceline(' + priceline.id + ', false)"> Submit Priceline Changes</button>' + // submit changes button
            '<button id="deletePriceline' + priceline.id + '" class="delete" onclick="deletePriceline(' + priceline.id + ', ' + priceline.itemID + ')"> Delete this Priceline</button> <br /> <br /> ' // delete priceline button
    }

    htmlString += '<br /> ';

    // if this is a create add to beginning of div
    if (create) {
        $(htmlString).prependTo($(PRICELINE_DIV_ID));
    }
    else {
        // otherwise add to end of div
        $(htmlString).appendTo($(PRICELINE_DIV_ID));
    }
}

function submitItem(create, menuID = 1) {
    // takes the values of the item input elements (id, name, description, and position)
    // and passes them to the api using create if true is passed and using edit if false is passed
    // second parameter is the menuID, used for the direct back to the display menu page (defaults to 1)

    // datavalidation check if position is a number
    let errorString = isPositiveNumber($('#position').val(), "Item Position");

    if (errorString == "") {


        // create item object by getting value attribute of the input elements
        let item = {
            "id": parseInt($('#id').val()),
            "name": $('#name').val(),
            "description": $('#description').val(),
            "position": parseFloat($('#position').val()),
            "sectionID": $('#sectionID').val(),
            "picturePath": $('#picturePath').val()
        };

        // request method is either POST (create) or PUT (edit)
        let methodString, operationString;
        if (create) {
            methodString = "POST";
            operationString = "created";
        }
        else {
            methodString = "PUT";
            operationString = "updated";
        }
        console.log(JSON.stringify(item));

        // ajax call
        $.ajax({
            method: methodString,               // either POST or PUT
            accepts: 'application/json',
            url: URL + "item",                  // url is api/item
            contentType: 'application/json',    // type of data being sent
            data: JSON.stringify(item),         // actual data being sent, use JSON library to convert the item object to JSON
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error: Item could not be ' + operationString);
            },
            success: function (result) {
                alert('Item successfully ' + operationString);
                window.location = "index.html?id=" + menuID;
            }
        });
    }
    else {
        // display errorString
        alert(errorString);
    }
}

function deleteItem(id) {
    // called by a delete button
    // first call changes the button's text to "Confirm Delete?"
    // second call sends an ajax request to delete that entry, then gets the items again

    // the confirm delete text
    let confirmDeleteText = "Confirm Delete, including all this item's prices?";

    // id of current button
    let buttonID = "#deleteItem" + id;

    if ($(buttonID).text() != confirmDeleteText) {
        // first call changes the text of the calling button
        $(buttonID).text(confirmDeleteText)
        console.log($(this).text());
    }
    else {
        // second call sends the ajax delete request

        // JSON to be sent, only requires the id
        let jsonString = '{ "id": ' + id + ' }';

        $.ajax({
            method: "DELETE",                     // method is DELETE
            accepts: 'application/json',
            url: URL + "item",                  // url is api/item
            contentType: 'application/json',    // type of data being sent
            data: jsonString,                   // actual data being sent
            success: function (result) {
                // Change button text
                $(buttonID).text("Successfully Deleted");

                // Remove this button's onclick event
                $(buttonID).prop('onclick', null).off('click');

                // refresh current menu
                showMenu(currentMenuID);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // Change button text to show delete failure
                $(buttonID).text("Delete Failed");
            }
        });
    }
}

function submitPriceline(pricelineID, create) {
    // takes a pricelineID and then grabs the values from the input elements (id, description, price, and position)
    // and passes them to the api using create if true is passed and using edit if false is passed

    // data validation: check if position and price are numbers
    let errorString = isPositiveNumber($('#pricelinePrice' + pricelineID).val(), "Priceline Price") + isPositiveNumber($('#pricelinePosition' + pricelineID).val(), "Priceline Position");

    if (errorString == "") {

        // create priceline object by getting value attribute of the input elements
        let priceline = {
            "id": parseInt($('#pricelineID' + pricelineID).val()),
            "description": $('#pricelineDescription' + pricelineID).val(),
            "price": parseFloat($('#pricelinePrice' + pricelineID).val()),
            "position": parseFloat($('#pricelinePosition' + pricelineID).val()),
            "itemID": $('#pricelineItemID' + pricelineID).val()
        };

        // request method is either POST (create) or PUT (edit)
        let methodString, operationString;
        if (create) {
            methodString = "POST";
            operationString = "created";
        }
        else {
            methodString = "PUT";
            operationString = "updated";
        }
        console.log(JSON.stringify(priceline));

        // ajax call
        $.ajax({
            method: methodString,               // either POST or PUT
            accepts: 'application/json',
            url: URL + "priceline",                  // url is api/priceline
            contentType: 'application/json',    // type of data being sent
            data: JSON.stringify(priceline),         // actual data being sent, use JSON library to convert the priceline object to JSON
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error: Priceline could not be ' + operationString);
            },
            success: function (result) {
                alert('Priceline successfully ' + operationString);

                // reload this item and its pricelines
                loadItem(priceline.itemID);
            }
        });
    }
    else {
        // display errorString
        alert(errorString);
    }
}

function deletePriceline(id, itemID) {
    // called by a delete button
    // first call changes the button's text to "Confirm Delete?"
    // second call sends an ajax request to delete that entry, then refreshes the item listing (id given by 2nd parameter)

    // the confirm delete text
    let confirmDeleteText = "Confirm Delete?";

    // id of current button
    let buttonID = "#deletePriceline" + id;

    if ($(buttonID).text() != confirmDeleteText) {
        // first call changes the text of the calling button
        $(buttonID).text(confirmDeleteText)
        console.log($(this).text());
    }
    else {
        // second call sends the ajax delete request

        // JSON to be sent, only requires the id
        let jsonString = '{ "id": ' + id + ' }';

        $.ajax({
            method: "DELETE",                     // method is DELETE
            accepts: 'application/json',
            url: URL + "priceline",                  // url is api/priceline
            contentType: 'application/json',    // type of data being sent
            data: jsonString,                   // actual data being sent
            success: function (result) {
                // Change button text
                $(buttonID).text("Successfully Deleted");

                // Remove this button's onclick event
                $(buttonID).prop('onclick', null).off('click');
                
                // reload the item and its pricelines
                loadItem(itemID);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // Change button text to show delete failure
                $(buttonID).text("Delete Failed");
            }
        });
    }
}

function isPositiveNumber(input, fieldName) {
    // takes an input and checks whether it's a positive number or not
    // second param is the name of the field being checked (used in error message)
    // returns empty string if true, an error message string if false

    let result = "";

    // check that's it's not null or an empty string, a number and greater than 0
    // isNaN() returns true if the input is not a number, false if it is
    // so get the opposite using not (!)
    let isNum = (input != null) && (input != "") && (!isNaN(input)) && parseFloat(input) > 0;

    if (isNum) {
        result = "";
    }
    else {
        result = "" + fieldName + " must be a positive number. \n";
    }
    return result;
}

