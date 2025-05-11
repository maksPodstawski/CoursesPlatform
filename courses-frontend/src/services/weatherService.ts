import { config } from '../config';

export const weatherService = {
    async getForecast() {
        const response = await fetch(`${config.apiBaseUrl}/api/weatherforecast`, {
            credentials: 'include'
        });

        if (!response.ok) {
            throw new Error('Failed to fetch weather forecast');
        }

        return response.json();
    }
}; 