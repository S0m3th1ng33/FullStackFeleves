function updateBackground(condition) {
    document.body.className = ""; // Reset previous background

    switch (condition) {
        case "Sunny":
            document.body.classList.add("sunny");
            break;
        case "Cloudy":
            document.body.classList.add("cloudy");
            break;
        case "Rain":
            document.body.classList.add("rainy");
            break;
        case "Snow":
            document.body.classList.add("snowy");
            break;
        default:
            document.body.classList.add("cloudy");
            break;
    }
}

function updateWeather(data, prefix) {
    const conditionElem = document.getElementById(`${prefix}-condition`);
    const temperatureElem = document.getElementById(`${prefix}-temperature`);
    const windElem = document.getElementById(`${prefix}-wind`);
    const iconElem = document.getElementById(`${prefix}-icon`);

    const roundedTemp = data.avgTempC.toFixed(1);
    const roundedWind = data.maxWindKph.toFixed(1);

    conditionElem.textContent = `Időjárás: ${data.condition}`;
    temperatureElem.textContent = `Hőmérséklet: ${roundedTemp} °C`;
    windElem.textContent = `Szélsebesség: ${roundedWind} km/h`;

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
        updateWeather(data.today, "today");
        updateWeather(data.forecast, "forecast");
    } catch (error) {
        alert("Nem sikerült lekérni az időjárási adatokat.");
        console.error("Hiba: ", error);
    }
}
document.addEventListener("DOMContentLoaded", getWeather);
