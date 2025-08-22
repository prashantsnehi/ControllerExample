
async function getPersons() {
    var response = await fetch("/home/GetPersons");
    console.log(response);
    var responseBody = await response.text();
    console.log(responseBody);
    document.getElementById('personslist').innerHTML = responseBody;
}

async function getPersonGrid() {
    var response = await fetch("/home/GetPersonGrid", { method: 'GET'});
    var responseBody = await response.text();
    document.getElementById("fromController").innerHTML = responseBody;
}