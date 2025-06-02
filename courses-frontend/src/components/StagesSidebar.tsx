import { useState } from "react";
import { CheckCircle, Lock, Play, Clock, Video, X, ChevronDown, ChevronUp } from "lucide-react";
import "../styles/StagesSidebar.css";

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

interface StagesSidebarProps {
	stages: Stage[];
	currentStageId: string;
	courseId: string;
	courseName: string;
	onStageSelect: (stageId: string) => void;
	onClose: () => void;
}

export function StagesSidebar({
	stages,
	currentStageId,
	courseId,
	courseName,
	onStageSelect,
	onClose,
}: StagesSidebarProps) {
	const [expandedStages, setExpandedStages] = useState<string[]>([currentStageId]);

	const toggleStageExpansion = (stageId: string) => {
		setExpandedStages((current) =>
			current.includes(stageId) ? current.filter((id) => id !== stageId) : [...current, stageId],
		);
	};

	const formatDuration = (minutes: number): string => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		if (hours > 0) {
			return `${hours}h ${mins}m`;
		}
		return `${mins}m`;
	};

	const completedStages = stages.filter((stage) => stage.isCompleted).length;
	const totalStages = stages.length;
	const progressPercent = (completedStages / totalStages) * 100;

	return (
		<div className="stages-sidebar">
			<div className="stages-sidebar__header">
				<div className="stages-sidebar__header-content">
					<div className="stages-sidebar__course-info">
						<h2 className="stages-sidebar__course-name">{courseName}</h2>
						<p className="stages-sidebar__course-subtitle">Course Stages</p>
					</div>
					<button className="stages-sidebar__close-btn" onClick={onClose}>
						<X size={16} />
					</button>
				</div>

				<div className="stages-sidebar__progress">
					<div className="stages-sidebar__progress-info">
						<span className="stages-sidebar__progress-label">Progress</span>
						<span className="stages-sidebar__progress-count">
							{completedStages}/{totalStages} stages
						</span>
					</div>
					<div className="stages-sidebar__progress-bar">
						<div className="stages-sidebar__progress-fill" style={{ width: `${progressPercent}%` }} />
					</div>
				</div>
			</div>

			<div className="stages-sidebar__content">
				<div className="stages-sidebar__stages-list">
					{stages.map((stage, index) => (
						<div key={stage.id} className="stages-sidebar__stage-item">
							<div
								className={`stages-sidebar__stage-main ${
									currentStageId === stage.id ? "current" : ""
								} ${stage.locked ? "locked" : ""}`}
								onClick={() => !stage.locked && onStageSelect(stage.id)}
								data-course-id={courseId}
							>
								<div className="stages-sidebar__stage-content">
									<div
										className={`stages-sidebar__stage-icon ${
											stage.isCompleted ? "completed" : stage.locked ? "locked" : "default"
										}`}
									>
										{stage.isCompleted ? (
											<CheckCircle size={16} />
										) : stage.locked ? (
											<Lock size={16} />
										) : (
											<Video size={16} />
										)}
									</div>

									<div className="stages-sidebar__stage-info">
										<div className="stages-sidebar__stage-header">
											<h3 className="stages-sidebar__stage-name">
												{index + 1}. {stage.name}
											</h3>
											{!stage.locked && (
												<button
													className="stages-sidebar__expand-btn"
													onClick={(e) => {
														e.stopPropagation();
														toggleStageExpansion(stage.id);
													}}
												>
													{expandedStages.includes(stage.id) ? <ChevronUp size={12} /> : <ChevronDown size={12} />}
												</button>
											)}
										</div>

										<div className="stages-sidebar__stage-meta">
											<Clock size={12} />
											<span>{formatDuration(stage.duration)}</span>
											{stage.isCompleted && (
												<>
													<span className="stages-sidebar__meta-separator">•</span>
													<CheckCircle size={12} />
													<span className="stages-sidebar__completed-text">Completed</span>
												</>
											)}
										</div>
									</div>
								</div>
							</div>

							{expandedStages.includes(stage.id) && !stage.locked && (
								<div className="stages-sidebar__stage-details">
									<p className="stages-sidebar__stage-description">{stage.description}</p>

									<div className="stages-sidebar__stage-actions">
										<div className="stages-sidebar__stage-features">
											{stage.videoPath && (
												<>
													<Video size={12} />
													<span>Video Available</span>
												</>
											)}
										</div>

										{currentStageId !== stage.id && (
											<button
												className={`stages-sidebar__action-btn ${stage.isCompleted ? "review" : "watch"}`}
												onClick={(e) => {
													e.stopPropagation();
													onStageSelect(stage.id);
												}}
											>
												<Play size={8} />
												{stage.isCompleted ? "Review" : "Watch"}
											</button>
										)}
									</div>
								</div>
							)}
						</div>
					))}
				</div>
			</div>
		</div>
	);
}
