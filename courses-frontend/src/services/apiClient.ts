import axios from 'axios';
import { config } from '../config';

export const apiClient = axios.create({
	baseURL: config.apiBaseUrl,
	withCredentials: true,
	headers: {
		'Content-Type': 'application/json'
	}
});
