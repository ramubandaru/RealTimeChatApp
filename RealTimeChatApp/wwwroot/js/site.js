$(document).ready(function () {
    var connection = new WebSocketManager.Connection("ws://localhost:5000/chat");
    connection.enableLogging = true;
    connection.start();
    connection.connectionMethods.onConnected = () => {
    }

    connection.connectionMethods.onDisconnected = () => {
        alert('Make sure the program is running or Refresh to connect to sockets');
    }


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
            if (typeof chatRoomId === "undefined") {
                alert('Select a channel to send a message');
            }
            else {
                connection.invoke("SendMessage", connection.connectionId, message, chatRoomId);
                $messageContent.val('');
            }

        }
    });

    var $messageHistory = $('#message-history');

    $('#chatRooms').change(function () {
        var chatRoomId = $("#chatRooms option:selected").val();
        //console.log(chatRoomId);
        
        if (chatRoomId >= 1) {
            $messageHistory.empty();
            connection.invoke("GetMessages", connection.connectionId, chatRoomId);
        }
    });

    $('#chatRooms').trigger('change');


    connection.clientMethods["pingMessage"] = (socketId, messageSentFrom, message, chatRoomId) => {
        //console.log('I got the message with socketId ' + socketId + ' and the message is ' + message);
      
        var messageText = messageSentFrom + ' : ' + message;
        
        $('#chatRooms').val(chatRoomId);
        $('#message-history').append('<li>' + messageText + '</li>');
        $('#message-history').scrollTop($('#message-history').prop('scrollHeight'));
       
    }

    

});
