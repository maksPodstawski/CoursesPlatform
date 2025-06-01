import { useRef, useState, useEffect } from "react";
import { Play, Pause, Volume2, VolumeX, Maximize, Minimize } from "lucide-react";
import { getStageVideoStreamUrl } from "../services/courseService";
import { startStage } from "../services/progressService";
import "../styles/VideoPlayer.css";

interface VideoPlayerProps {
	stageId: string;
	stageName: string;
	onComplete?: () => void;
	autoplay?: boolean;
}

export function VideoPlayer({ stageId, stageName, onComplete, autoplay = false }: VideoPlayerProps) {
	const videoRef = useRef<HTMLVideoElement>(null);
	const [isPlaying, setIsPlaying] = useState(false);
	const [isMuted, setIsMuted] = useState(false);
	const [isFullscreen, setIsFullscreen] = useState(false);
	const [progress, setProgress] = useState(0);
	const [duration, setDuration] = useState(0);
	const [currentTime, setCurrentTime] = useState(0);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		const video = videoRef.current;
		if (!video) return;

		const handleTimeUpdate = () => {
			setProgress((video.currentTime / video.duration) * 100);
			setCurrentTime(video.currentTime);
		};

		const handleLoadedMetadata = () => {
			setDuration(video.duration);
		};

		const handleEnded = () => {
			setIsPlaying(false);
			onComplete?.();
		};

		const handleError = (e: ErrorEvent) => {
			console.error("Video error:", e);
			setError("Failed to load video. Please try again later.");
		};

		video.addEventListener("timeupdate", handleTimeUpdate);
		video.addEventListener("loadedmetadata", handleLoadedMetadata);
		video.addEventListener("ended", handleEnded);
		video.addEventListener("error", handleError as EventListener);

		return () => {
			video.removeEventListener("timeupdate", handleTimeUpdate);
			video.removeEventListener("loadedmetadata", handleLoadedMetadata);
			video.removeEventListener("ended", handleEnded);
			video.removeEventListener("error", handleError as EventListener);
		};
	}, [onComplete]);

	useEffect(() => {
		const video = videoRef.current;
		if (!video) return;

		if (autoplay) {
			video.play().catch(error => {
				console.error("Autoplay failed:", error);
				setError("Autoplay is not allowed. Please click play to start the video.");
			});
		}
	}, [autoplay]);

	const handlePlay = async () => {
		try {
			await startStage(stageId);
		} catch (error) {
			console.error("Error starting stage:", error);
		}
	};

	const togglePlay = () => {
		const video = videoRef.current;
		if (!video) return;

		if (isPlaying) {
			video.pause();
		} else {
			video.play()
				.then(() => handlePlay())
				.catch(error => {
					console.error("Play failed:", error);
					setError("Failed to play video. Please try again.");
				});
		}
		setIsPlaying(!isPlaying);
	};

	const toggleMute = () => {
		const video = videoRef.current;
		if (!video) return;

		video.muted = !isMuted;
		setIsMuted(!isMuted);
	};

	const toggleFullscreen = () => {
		if (!document.fullscreenElement) {
			videoRef.current?.requestFullscreen();
			setIsFullscreen(true);
		} else {
			document.exitFullscreen();
			setIsFullscreen(false);
		}
	};

	const handleProgressClick = (e: React.MouseEvent<HTMLDivElement>) => {
		const video = videoRef.current;
		if (!video) return;

		const rect = e.currentTarget.getBoundingClientRect();
		const pos = (e.clientX - rect.left) / rect.width;
		video.currentTime = pos * video.duration;
	};

	const formatTime = (time: number): string => {
		const minutes = Math.floor(time / 60);
		const seconds = Math.floor(time % 60);
		return `${minutes}:${seconds.toString().padStart(2, "0")}`;
	};

	if (error) {
		return (
			<div className="video-player__error">
				<p>{error}</p>
				<button onClick={() => setError(null)}>Try Again</button>
			</div>
		);
	}

	return (
		<div className="video-player">
			<video
				ref={videoRef}
				className="video-player__video"
				src={getStageVideoStreamUrl(stageId)}
				poster={`/api/stages/${stageId}/thumbnail`}
				onClick={togglePlay}
				controlsList="nodownload"
			>
				Your browser does not support the video tag.
			</video>

			<div className="video-player__controls">
				<button className="video-player__control-btn" onClick={togglePlay}>
					{isPlaying ? <Pause size={20} /> : <Play size={20} />}
				</button>

				<div className="video-player__progress-container" onClick={handleProgressClick}>
					<div className="video-player__progress-bar">
						<div
							className="video-player__progress-fill"
							style={{ width: `${progress}%` }}
						/>
					</div>
				</div>

				<div className="video-player__time">
					{formatTime(currentTime)} / {formatTime(duration)}
				</div>

				<button className="video-player__control-btn" onClick={toggleMute}>
					{isMuted ? <VolumeX size={20} /> : <Volume2 size={20} />}
				</button>

				<button className="video-player__control-btn" onClick={toggleFullscreen}>
					{isFullscreen ? <Minimize size={20} /> : <Maximize size={20} />}
				</button>
			</div>
		</div>
	);
} 