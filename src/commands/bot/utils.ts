import * as discord from 'discord.js';
import { MessageEmbed } from 'discord.js';
import { CommandoClient, CommandoMessage } from 'discord.js-commando';
import { pool } from '../../db/db';

export async function getMember(uid: string, guild: discord.Guild) {
  let uidParsed = uid;
  // Check if user was tagged or not. If the user was tagged remove the
  // tag from id.
  if (uid.startsWith('<@') && uid.endsWith('>')) {
    const re = new RegExp('[<@!>]', 'g');
    uidParsed = uid.replace(re, '');
  }
  // Try recovering the user and report if it was successful or not.
  try {
    return await guild.members.fetch(uidParsed);
  } catch (e) {
    console.log(`User not found because ${e}`);
    return undefined;
  }
}

export async function getChannel(uid: string, client: CommandoClient) {
  let uidParsed = uid;
  // Check if user was tagged or not. If the user was tagged remove the
  // tag from id.
  if (uid.startsWith('<#') && uid.endsWith('>')) {
    const re = new RegExp('[<#!>]', 'g');
    uidParsed = uid.replace(re, '');
  }
  // Try recovering the user and report if it was successful or not.
  try {
    return await client.channels.fetch(uidParsed, true, true);
  } catch (e) {
    console.log(`User not found because ${e}`);
    return undefined;
  }
}

export function sleep(ms: number) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

export async function checkIfUserMuted(userId: string) {
  const res = await pool.query('SELECT * FROM anon_muting.users WHERE user_id = $1', [userId]);
  return !(res.rowCount === 0 || !res.rows[0].muted);
}

export function getMuteReadableTime(offence: number) {
  switch (offence) {
    case 1:
      return 'for 7 days';
    case 2:
      return 'for 14 days';
    case 3:
      return 'for 1 month';
    default:
      return 'permanently';
  }
}

export function getFileExtension(filename: string) {
  return filename.substr(filename.lastIndexOf('.') + 1);
}

export async function selectRestriction(msg: CommandoMessage): Promise<number> {
  // 0: No restrictions 🟢
  // 1: image only 📷
  // 2: gif/mp4 only 🎥
  // 3: mp4/gif/image only 📸
  // 4: text only 📖

  const embed = new MessageEmbed({
    title: 'Select restriction',
    color: 'BLURPLE',
    description: '__**Options:**__\n\n'
            + '🟢: **No restrictions**\n'
            + '📷: **Image only**.\n'
            + '🎥: **Gif/mp4 only.**\n'
            + '📸: **Require attachment.**\n'
            + '📖: **No attachments allowed, text only.**',
  });

  const embedMsg = await msg.channel.send(embed);

  const emotes = ['🟢', '📷', '🎥', '📸', '📖'];

  for (const emote of emotes) {
    await embedMsg.react(emote);
  }

  const collected = await embedMsg.awaitReactions((reaction, user) => emotes.includes(reaction.emoji.name) && user.id === msg.author.id,
    { max: 1, time: 60000, errors: ['time'] });

  const reaction = collected.first();
  if (reaction === undefined) {
    console.error('How tf did you get here');
    return 0;
  }

  switch (reaction.emoji.name) {
    case '🟢':
      return 0;
    case '📷':
      return 1;
    case '🎥':
      return 2;
    case '📸':
      return 3;
    case '📖':
      return 4;
    default:
      return 0;
  }
}
