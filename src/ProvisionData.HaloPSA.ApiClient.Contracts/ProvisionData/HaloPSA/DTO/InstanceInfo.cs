// Provision Data HaloPSA API Client
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

namespace ProvisionData.HaloPSA.DTO;

public record InstanceInfo(
    [property: JsonPropertyName("dbid")] String ID,
    [property: JsonPropertyName("app_version")] String AppVersion,
    [property: JsonPropertyName("database_version")] String DatabaseVersion,
    [property: JsonPropertyName("agent_count")] Int32 AgentCount,
    [property: JsonPropertyName("licensed_agent_count")] Int32 LicensedAgentCount,
    [property: JsonPropertyName("online_agent_count")] Int32 OnlineAgentCount,
    [property: JsonPropertyName("last_login")] DateTime LastLogin,
    [property: JsonPropertyName("app_name")] String AppName,
    [property: JsonPropertyName("agent_url")] String AgentUrl,
    [property: JsonPropertyName("portal_url")] String PortalUrl,
    [property: JsonPropertyName("auth_url")] String AuthUrl,
    [property: JsonPropertyName("mobileapp_enabled")] Boolean MobileappEnabled,
    [property: JsonPropertyName("teamsbot_enabled")] Boolean TeamsbotEnabled,
    [property: JsonPropertyName("nhserver_version")] String NhserverVersion,
    [property: JsonPropertyName("teams_chat_profile")] String TeamsChatProfile,
    [property: JsonPropertyName("using_event_service")] Boolean UsingEventService,
    [property: JsonPropertyName("using_scheduling_service")] Boolean UsingSchedulingService,
    [property: JsonPropertyName("using_outgoing_service")] Boolean UsingOutgoingService,
    [property: JsonPropertyName("using_incoming_service")] Boolean UsingIncomingService
);

