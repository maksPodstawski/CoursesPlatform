import { fetchClient } from "./fetchClient";
import { config } from "../config";

export const invitationService = {
  async getInvitationsByEmail(email: string) {
    const response = await fetchClient.fetch(`/api/invitation/by-email?email=${encodeURIComponent(email)}`);
    if (!response.ok) throw new Error("Failed to fetch invitations");
    return response.json();
  },

  async acceptInvitation(invitationId: string) {
    const response = await fetchClient.fetch(`/api/invitation/accept`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ invitationId }),
    });
    if (!response.ok) throw new Error("Failed to accept invitation");
    return response;
  },

  async declineInvitation(invitationId: string) {
    const response = await fetchClient.fetch(`/api/invitation/decline`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ invitationId }),
    });
    if (!response.ok) throw new Error("Failed to decline invitation");
    return response;
  },

  async inviteByEmail(email: string, courseId: string) {
    console.log("InvitationService: Sending invitation", { email, courseId });
    
    // Dodaj retry logic dla problemów z DbContext
    const maxRetries = 3;
    let lastError: Error | null = null;
    
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        console.log(`InvitationService: Attempt ${attempt}/${maxRetries}`);
        
        const response = await fetchClient.fetch(config.apiEndpoints.inviteByEmail, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ 
            email, 
            courseId: courseId
          }),
        });
        
        if (response.ok) {
          console.log("InvitationService: Success");
          return response;
        }
        
        const errorText = await response.text();
        console.error(`InvitationService: Error response (attempt ${attempt})`, errorText);
        
        // Jeśli to błąd DbContext, spróbuj ponownie
        if (errorText.includes("DbContext") || errorText.includes("second operation")) {
          lastError = new Error(errorText);
          if (attempt < maxRetries) {
            console.log("DbContext error detected, retrying...");
            await new Promise(resolve => setTimeout(resolve, 2000 * attempt)); // Exponential backoff
            continue;
          }
        }
        
        throw new Error(errorText || "Failed to send invitation");
      } catch (err: any) {
        lastError = err;
        console.error(`InvitationService: Exception (attempt ${attempt})`, err);
        
        if (attempt < maxRetries) {
          await new Promise(resolve => setTimeout(resolve, 2000 * attempt));
          continue;
        }
      }
    }
    
    throw lastError || new Error("Failed to send invitation after all retries");
  },
}; 