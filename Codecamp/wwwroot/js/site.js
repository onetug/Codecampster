// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function setSpeakerImage(img) {
    let url = location.origin + "/api/Speakers/image/" + img.id;
    fetch(url)
        .then(function (response) {
            return response.json();
        })
        .then(function (data) {
            img.src = data.imageSrc;
        })
        .catch(function (err) {
            console.log(err);
        });
}