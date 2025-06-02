import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { CheckCircle, Clock, MessageSquare, List, ChevronLeft, ChevronRight } from "lucide-react";
import { StagesSidebar } from "./StagesSidebar";
import { StageChat } from "./StageChat";
import { VideoPlayer } from "./VideoPlayer";
import "../styles/StagePlayer.css";

interface Stage {
	id: string;
	courseId: string;
	name: string;
	description: string;
	duration: number;
	videoPath?: string;
	createdAt: string;
	isCompleted: boolean;
	locked: boolean;
}

interface StagePlayerProps {
	stage: Stage;
	courseId: string;
	courseName: string;
	allStages: Stage[];
	nextStageId?: string;
	prevStageId?: string;
	onComplete?: () => void;
}

export function StagePlayer({
	stage,
	courseId,
	courseName,
	allStages,
	nextStageId,
	prevStageId,
	onComplete,
}: StagePlayerProps) {
	const navigate = useNavigate();
	const [isCompleted, setIsCompleted] = useState(stage.isCompleted);
	const [showStagesSidebar, setShowStagesSidebar] = useState(true);
	const [showChat, setShowChat] = useState(true);
	const [sidebarPosition, setSidebarPosition] = useState<"left" | "right">("left");

	const handleStageComplete = () => {
		setIsCompleted(true);
		console.log(`Stage ${stage.id} completed`);
		onComplete?.();
	};

	const navigateToStage = (stageId: string) => {
		navigate(`/course/${courseId}/stage/${stageId}`);
	};

	const navigateToCourse = () => {
		navigate(`/course/${courseId}`);
	};

	
	const formatDuration = (minutes: number): string => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		if (hours > 0) {
			return `${hours}h ${mins}m`;
		}
		return `${mins}m`;
	};

	return (
		<div className="stage-player">
			{showStagesSidebar && sidebarPosition === "left" && (
				<div className="stage-player__sidebar stage-player__sidebar--left">
					<StagesSidebar
						stages={allStages}
						currentStageId={stage.id}
						courseId={courseId}
						courseName={courseName}
						onStageSelect={navigateToStage}
						onClose={() => setShowStagesSidebar(false)}
					/>
					<div className="stage-player__sidebar-nav-buttons">
						<button
							className="stage-player__nav-btn stage-player__nav-btn--prev"
							onClick={() => prevStageId && navigateToStage(prevStageId)}
							disabled={!prevStageId}
						>
							<ChevronLeft className="stage-player__icon" />
							Previous
						</button>

						<button
							className="stage-player__nav-btn stage-player__nav-btn--next"
							onClick={() => nextStageId && navigateToStage(nextStageId)}
							disabled={!nextStageId}
						>
							Next
							<ChevronRight className="stage-player__icon" />
						</button>
					</div>
				</div>
			)}

			<div className="stage-player__main">
				<div className="stage-player__header">
					<div className="stage-player__header-content">
						<div className="stage-player__navigation">
							<button className="stage-player__back-btn" onClick={navigateToCourse}>
								<ChevronLeft className="stage-player__icon" />
								Back to {courseName}
							</button>

							<div className="stage-player__controls">
								<button className="stage-player__control-btn" onClick={() => setShowStagesSidebar(!showStagesSidebar)}>
									<List className="stage-player__icon" />
									Stages
								</button>
								<button className="stage-player__control-btn" onClick={() => setShowChat(!showChat)}>
									<MessageSquare className="stage-player__icon" />
									Chat
								</button>
								<button
									className="stage-player__control-btn"
									onClick={() => setSidebarPosition(sidebarPosition === "left" ? "right" : "left")}
								>
									<ChevronRight className="stage-player__icon" />
									Switch Sides
								</button>
							</div>
						</div>

						<div className="stage-player__status">
							{isCompleted && (
								<div className="stage-player__badge stage-player__badge--completed">
									<CheckCircle className="stage-player__icon" />
									Completed
								</div>
							)}
						</div>
					</div>
				</div>

				<div className="stage-player__info">
					<h1 className="stage-player__title">{stage.name}</h1>
					<p className="stage-player__description">{stage.description}</p>

					<div className="stage-player__meta">
						<div className="stage-player__meta-item">
							<Clock className="stage-player__icon" />
							<span>{formatDuration(stage.duration)}</span>
						</div>
					</div>
				</div>

				<div className="stage-player__video-container">
					{stage.id ? (
						<VideoPlayer stageId={stage.id} stageName={stage.name} onComplete={handleStageComplete} autoplay={false} />
					) : (
						<div className="stage-player__no-video">
							<p>No video content available for this stage.</p>
							<p className="stage-player__no-video-subtitle">
								Please contact the course instructor if you believe this is an error.
							</p>
						</div>
					)}
				</div>
			</div>

			{showChat && (
				<div className="stage-player__sidebar stage-player__sidebar--right">
					<StageChat
						stageId={stage.id}
						stageName={stage.name}
						courseId={courseId}
						courseName={courseName}
						onClose={() => setShowChat(false)}
					/>
				</div>
			)}

			{showStagesSidebar && sidebarPosition === "right" && (
				<div className="stage-player__sidebar stage-player__sidebar--right">
					<StagesSidebar
						stages={allStages}
						currentStageId={stage.id}
						courseId={courseId}
						courseName={courseName}
						onStageSelect={navigateToStage}
						onClose={() => setShowStagesSidebar(false)}
					/>
				</div>
			)}
		</div>
	);
}
