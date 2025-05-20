function updateBackground(condition) {
    const body = document.body;

    // Először töröljük az összes időjáráshoz tartozó osztályt, de meghagyjuk a dark-mode-ot, ha van
    const preserved = body.classList.contains("dark-mode") ? ["dark-mode"] : [];

    body.className = preserved.join(" "); // csak a dark-mode marad, ha volt

    switch (condition) {
        case "Sunny":
        case "Clear":
            body.classList.add("sunny");
            break;
        case "Partly cloudy":
        case "Cloudy":
        case "Overcast":
            body.classList.add("cloudy");
            break;
        case "Mist":
        case "Fog":
        case "Freezing fog":
            body.classList.add("foggy");
            break;
        case "Patchy rain possible":
        case "Patchy light drizzle":
        case "Patchy light rain":
        case "Light drizzle":
        case "Light rain":
        case "Rain":
        case "Heavy rain":
        case "Moderate rain":
            body.classList.add("rainy");
            break;
        case "Thunderstorm":
        case "Patchy light rain with thunder":
        case "Moderate or heavy rain with thunder":
            body.classList.add("thunderstorm");
            break;
        case "Snow":
        case "Light snow":
        case "Heavy snow":
        case "Patchy snow":
        case "Blizzard":
        case "Snow shower":
            body.classList.add("snowy");
            break;
        case "Sleet":
        case "Light sleet":
        case "Moderate sleet":
            body.classList.add("sleety");
            break;
        case "Hail":
        case "Moderate or heavy hail":
            body.classList.add("hail");
            break;
        default:
            body.classList.add("default-weather");
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

    document.getElementById(`${prefix}-condition`).textContent = `Időjárás: ${data.condition}`;
    document.getElementById(`${prefix}-temperature`).textContent = `Hőmérséklet: ${data.avgTempC.toFixed(1)} °C`;
    document.getElementById(`${prefix}-wind`).textContent = `Szélsebesség: ${data.maxWindKph.toFixed(1)} km/h`;
    if (data.icon) {
        const iconElem = document.getElementById(`${prefix}-icon`);
        iconElem.src = data.icon;
        iconElem.alt = data.condition;
    }
    document.getElementById(`today-front`).textContent = `Várható front: ${data.frontInfo}`;
    if (prefix === "today") {
        const moonPhase = data.moonPhase;
        const moonIconMap = {
            "New Moon": "https://img.icons8.com/?size=100&id=Wdnu-edbShJS&format=png&color=000000",
            "Waxing Crescent": "https://img.icons8.com/?size=100&id=CHn0rtZuD2M0&format=png&color=000000",
            "First Quarter": "https://img.icons8.com/?size=100&id=cy8DHBgUJqqL&format=png&color=000000",
            "Waxing Gibbous": "https://img.icons8.com/?size=100&id=SnlxFjy7u-4t&format=png&color=000000",
            "Full Moon": "https://img.icons8.com/?size=100&id=NJx6Gbc4Ng7C&format=png&color=000000",
            "Waning Gibbous": "https://img.icons8.com/?size=100&id=RLniTqU8gD1y&format=png&color=000000",
            "Last Quarter": "https://img.icons8.com/?size=100&id=KIPHVfQWWl4R&format=png&color=000000",
            "Waning Crescent": "https://img.icons8.com/?size=100&id=JGGPnA5MB09j&format=png&color=000000"
        };
        const moonIcon = document.getElementById("moon-icon");
        if (moonIcon && moonPhase in moonIconMap) {
            moonIcon.src = moonIconMap[moonPhase];
            moonIcon.alt = moonPhase;
        }
        document.getElementById("sunrise").textContent = `Napkelte: ${data.sunrise}`;
        document.getElementById("sunset").textContent = `Napnyugta: ${data.sunset}`;
        document.getElementById("moonrise").textContent = `Holdkelte: ${data.moonrise}`;
        document.getElementById("moonset").textContent = `Holdnyugta: ${data.moonset}`;
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
