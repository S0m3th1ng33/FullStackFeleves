function updateBackground(condition) {
    document.body.className = ""; // Reset previous background

    switch (condition) {
        case "Sunny":
        case "Clear":
            document.body.classList.add("sunny");
            break;
        case "Partly cloudy":
        case "Cloudy":
        case "Overcast":
            document.body.classList.add("cloudy");
            break;
        case "Mist":
        case "Fog":
        case "Freezing fog":
            document.body.classList.add("foggy");
            break;
        case "Patchy rain possible":
        case "Patchy light drizzle":
        case "Patchy light rain":
        case "Light drizzle":
        case "Light rain":
        case "Rain":
        case "Heavy rain":
        case "Moderate rain":
            document.body.classList.add("rainy");
            break;
        case "Thunderstorm":
        case "Patchy light rain with thunder":
        case "Moderate or heavy rain with thunder":
            document.body.classList.add("thunderstorm");
            break;
        case "Snow":
        case "Light snow":
        case "Heavy snow":
        case "Patchy snow":
        case "Blizzard":
        case "Snow shower":
            document.body.classList.add("snowy");
            break;
        case "Sleet":
        case "Light sleet":
        case "Moderate sleet":
            document.body.classList.add("sleety");
            break;
        case "Hail":
        case "Moderate or heavy hail":
            document.body.classList.add("hail");
            break;
        default:
            document.body.classList.add("default-weather");
            break;
    }
}

function toggleDarkMode() {
    const body = document.body;
    const modeIcon = document.getElementById("mode-icon");
    body.classList.toggle("dark-mode");

    if (body.classList.contains("dark-mode")) {
        modeIcon.src = "https://cdn-icons-png.flaticon.com/512/1164/1164954.png";
        modeIcon.alt = "Light Mode";
    } else {
        modeIcon.src = "https://cdn-icons-png.flaticon.com/512/667/667421.png";
        modeIcon.alt = "Dark Mode";
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
