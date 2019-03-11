var SessionUpdater = (function () {
    var clientMovedSinceLastTimeout = false;
    var keepSessionAliveUrl = null;
    var timeout = 5 * 1000 * 60; // 5 minutes

    function setupSessionUpdater(actionUrl) {
        // store local value
        keepSessionAliveUrl = actionUrl;
        clientMovedSinceLastTimeout = true;
        // setup handlers
        //listenForChanges();
        // start timeout - it'll run after n minutes
        checkToKeepSessionAlive();
    }

    function listenForChanges() {
        $("body").one("mousemove keydown", function () {
            clientMovedSinceLastTimeout = true;
        });
    }


    // fires every n minutes - if there's been movement ping server and restart timer
    function checkToKeepSessionAlive() {
        setTimeout(function () { keepSessionAlive(); }, timeout);
    }

    function keepSessionAlive() {
        // if we've had any movement since last run, ping the server
        if (clientMovedSinceLastTimeout && keepSessionAliveUrl != null) {
            console.log("session URL: " + keepSessionAliveUrl);
            $.ajax({
                type: "POST",
                url:  keepSessionAliveUrl,
                success: function (data) {
                    // reset movement flag
                    clientMovedSinceLastTimeout = true;
                    // start listening for changes again
                    //listenForChanges();
                    // restart timeout to check again in n minutes
                    checkToKeepSessionAlive();


                },
                error: function (data) {
                    
                    console.log("Error posting to " & keepSessionAliveUrl);
                    clientMovedSinceLastTimeout = true;
                    checkToKeepSessionAlive();
                }
            });
        }
    }

    // export setup method
    return {
        Setup: setupSessionUpdater
    };

})();