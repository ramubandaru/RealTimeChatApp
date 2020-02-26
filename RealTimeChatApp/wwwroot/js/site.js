$(document).ready(function () {
    var connection = new WebSocketManager.Connection("ws://localhost:5000/chat");
    connection.enableLogging = true;

    connection.connectionMethods.onConnected = () => {

        
    }

    connection.connectionMethods.onDisconnected = () => {

        alert('Make sure the program is running or Refresh to connect to sockets');
    }

    connection.clientMethods["pingMessage"] = (socketId, message) => {
        //console.log('I got the message with socketId ' + SocketId + ' and the message is ' + Message + ' for group ');
        console.log('I got the message with socketId ' + socketId + ' and the message is ' + message);
        var messageText = socketId + ' said: ' + Message;
        $('#message-history').append('<li>' + messageText + '</li>');
        $('#message-history').scrollTop($('#message-history').prop('scrollHeight'));
        
    }

    connection.start();



    var $messageContent = $('#message-content');
    $messageContent.keyup(function (e) {
        if (e.keyCode == 13) {
            var message = $messageContent.val().trim();
            if (message.length == 0) {
                return false;
            }

            //var roomSelected = $('#chatRooms option:selected').text();
            var chatRoomId = $("#chatRooms option:selected").val();
            console.log(chatRoomId);
            connection.invoke("SendMessage", connection.connectionId, message, chatRoomId);
            $messageContent.val('');

        }
    });


    //$('#chatRooms').change(function () {
    //    var chatRoomId = $("#chatRooms option:selected").val();
    //    console.log(chatRoomId);
    //    if (chatRoomId >= 1) {
    //        connection.invoke("GetMessages", connection.connectionId, chatRoomId);
    //    }
        

    //});

    //$('#chatRooms').trigger('change');

    
        
  



    //// And now fire change event when the DOM is ready
    //$('#chatRooms').trigger('change');


    //var sel = document.getElementById('chatRooms');

    //function fireEvent(element, event) {
    //        var evt = document.createEvent("HTMLEvents");
    //        evt.initEvent(event, true, true); 
    //        return !element.dispatchEvent(evt);
    //    }
    //}

 


    //var $chatroomselected = $('chatroom');
    //$chatroomselected.select(function (e) {
    //    console.log('chatroom selected');
    //    var roomselected = $chatroomselected.val().trim();

    //    connection.invoke("LoadRoomChat", connection.connectionId);


    //}


    //    $('#cmbMoreFunction').change(function () {
    //        var selectedValue = parseInt(jQuery(this).val());

    //        //Depend on Value i.e. 0 or 1 respective function gets called. 
    //        switch (selectedValue) {
    //            case 0:
    //                handlerFunctionA();
    //                break;
    //            case 1:
    //                handlerFunctionB();
    //                break;
    //            //etc... 
    //            default:
    //                alert("catch default");
    //                break;
    //        }
    //    });

    //function handlerFunctionA() {
    //    connection.invoke("SendMessage", connection.connectionId, message, parseInt(jQuery(this).val()));
    //}

    //function handlerFunctionB() {
    //    alert("Do some other stuff");
    //}


});
