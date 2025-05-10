function updateBackground(condition) {
    document.body.className = ""; // Reset previous background

    switch (condition) {
        case "napos":
            document.body.classList.add("sunny");
            break;
        case "borult":
            document.body.classList.add("cloudy");
            break;
        case "esős":
            document.body.classList.add("rainy");
            break;
        case "havas":
            document.body.classList.add("snowy");
            break;
        default:
            document.body.classList.add("cloudy");
            break;
    }
}

function updateWeather(data) {
    const conditionElem = document.getElementById("condition");
    const temperatureElem = document.getElementById("temperature");
    const windElem = document.getElementById("wind");
    const iconElem = document.getElementById("weather-icon");

    conditionElem.textContent = `Időjárás: ${data.condition}`;
    temperatureElem.textContent = `Hőmérséklet: ${data.avgTempC} °C`;
    windElem.textContent = `Szélsebesség: ${data.maxWindKph} km/h`;

    if (data.icon) {
        iconElem.src = data.icon;
        iconElem.alt = data.condition;
    }

    updateBackground(data.condition);
}

async function getWeather() {
    try {
        const response = await fetch("http://localhost:5197/WeatherForecast");
        if (!response.ok) throw new Error("Hiba az adatlekérés során");

        const data = await response.json();
        updateWeather(data);
    } catch (error) {
        alert("Nem sikerült lekérni az időjárási adatokat.");
        console.error("Hiba: ", error);
    }
}

document.addEventListener("DOMContentLoaded", getWeather);
