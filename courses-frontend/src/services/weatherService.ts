import { fetchClient } from './fetchClient';

export const weatherService = {
    async getForecast() {
        const response = await fetchClient.fetch('/api/weatherforecast');
        return response.json();
    }
}; 