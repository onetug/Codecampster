// This javascript is to be set to an event
// It decorates the sessionize list with a 'favorite' button to the session

function favoriteSession(id) {
    console.log('session favorited', id);
}

window.addEventListener('sessionize.onload', function (e) {

    var sessionDescriptions = document.getElementsByClassName("sz-session__description");
    if (sessionDescriptions) {
        for (i = 0; i < sessionDescriptions.length; i++) {
            var sessionDescription = sessionDescriptions[i];
            var sessionId = sessionDescription.parentNode.id;
            var favoriteLink = document.createElement("a");
            var content = document.createTextNode("Favorite");
            favoriteLink.setAttribute('class', 'signature');
            favoriteLink.setAttribute('href', `javascript:favoriteSession('${sessionId}')`);
            favoriteLink.appendChild(content);
            sessionDescription.parentNode.insertBefore(favoriteLink, sessionDescription);
        }
    }
});
